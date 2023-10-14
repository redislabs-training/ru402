using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;
using static NRedisStack.Search.Schema;

using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms.Text;
using System.Text.Json;


namespace Samples.Dynamic
{
    public static class ApplyWordEmbedding
    {

        static void Main() {
            CreateIndex();
            Example();
            TestSentence();
        }

        private static void CreateIndex(){
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

            var schema = new Schema()
            .AddTextField(new FieldName("content", "content"))
            .AddTagField(new FieldName("genre", "genre"))
            .AddVectorField("embedding", VectorField.VectorAlgo.HNSW,
                new Dictionary<string, object>()
                {
                    ["TYPE"] = "FLOAT32",
                    ["DIM"] = "150",
                    ["DISTANCE_METRIC"] = "L2"
                }
            );

            SearchCommands ft = db.FT();
            ft.Create(
                "vector_idx",
                new FTCreateParams().On(IndexDataType.HASH).Prefix("doc:"),
                schema);
        }

        private static byte[] GetEmbedding(PredictionEngine<TextData, TransformedTextData> model, string sentence)
        {
            // Call the prediction API to convert the text into embedding vector.
            var data = new TextData()
            {
                Text = sentence
            };
            var prediction = model.Predict(data);

            // Convert prediction.Features to a binary blob
            float[] floatArray = Array.ConvertAll(prediction.Features, x => (float)x);
            byte[] byteArray = new byte[floatArray.Length * sizeof(float)];
            Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);

            return byteArray;
        }

        private static PredictionEngine<TextData, TransformedTextData> GetPredictionEngine(){
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

            // Create a new ML context, for ML.NET operations. It can be used for
            // exception tracking and logging, as well as the source of randomness.
            var mlContext = new MLContext();

            // Create an empty list as the dataset
            var emptySamples = new List<TextData>();

            // Convert sample list to an empty IDataView.
            var emptyDataView = mlContext.Data.LoadFromEnumerable(emptySamples);

            // A pipeline for converting text into a 150-dimension embedding vector
            var textPipeline = mlContext.Transforms.Text.NormalizeText("Text")
                .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens",
                    "Text"))
                .Append(mlContext.Transforms.Text.ApplyWordEmbedding("Features",
                    "Tokens", WordEmbeddingEstimator.PretrainedModelKind
                    .SentimentSpecificWordEmbedding));

            // Fit to data.
            var textTransformer = textPipeline.Fit(emptyDataView);

            // Create the prediction engine to get the embedding vector from the input text/string.
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TextData,
                TransformedTextData>(textTransformer);

            return predictionEngine;
        }
        
        public static void Example()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

           var predictionEngine = GetPredictionEngine();

            // Create data
            var hash1 = new HashEntry[] { 
                new HashEntry("content", "That is a very happy person"), 
                new HashEntry("genre", "persons"),
                new HashEntry("embedding", GetEmbedding(predictionEngine, "That is a very happy person")),
            };
            db.HashSet("doc:1", hash1);

            var hash2 = new HashEntry[] { 
                new HashEntry("content", "That is a happy dog"), 
                new HashEntry("genre", "pets"),
                new HashEntry("embedding", GetEmbedding(predictionEngine, "That is a happy dog")),
            };
            db.HashSet("doc:2", hash2);

            var hash3 = new HashEntry[] { 
                new HashEntry("content", "Today is a sunny day"), 
                new HashEntry("genre", "weather"),
                new HashEntry("embedding", GetEmbedding(predictionEngine, "Today is a sunny day")),
            };
            db.HashSet("doc:3", hash3);
        }

        private static void TestSentence(){
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            var predictionEngine = GetPredictionEngine();

            SearchCommands ft = db.FT();
            var res = ft.Search("vector_idx",
                        new Query("*=>[KNN 3 @embedding $query_vec AS score]")
                        .AddParam("query_vec", GetEmbedding(predictionEngine, "That is a happy person"))
                        .SetSortBy("score")
                        .Dialect(2));

            foreach (var doc in res.Documents) {
                foreach (var item in doc.GetProperties()) {
                    if (item.Key == "score") {
                        Console.WriteLine($"id: {doc.Id}, score: {item.Value}");
                    }
                }
            }
        }

        private class TextData
        {
            public string Text { get; set; }
        }

        private class TransformedTextData : TextData
        {
            public float[] Features { get; set; }
        }
    }
}
