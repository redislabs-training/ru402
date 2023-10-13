package com.redis.app;
import redis.clients.jedis.Jedis;
import redis.clients.jedis.UnifiedJedis;
import redis.clients.jedis.search.*;
import redis.clients.jedis.search.schemafields.*;
import redis.clients.jedis.HostAndPort;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Map;
import java.util.HashMap;
import java.util.Arrays;
import java.util.List;

import ai.djl.huggingface.tokenizers.Encoding;
import ai.djl.huggingface.tokenizers.HuggingFaceTokenizer;
import ai.djl.inference.Predictor;
import ai.djl.modality.cv.Image;
import ai.djl.modality.cv.ImageFactory;
import ai.djl.modality.cv.translator.ImageFeatureExtractor;
import ai.djl.repository.zoo.ZooModel;
import ai.djl.translate.Pipeline;
import ai.djl.translate.TranslateException;


public class App {
    public static byte[] floatArrayToByteArray(float[] input) {
        byte[] bytes = new byte[Float.BYTES * input.length];
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).asFloatBuffer().put(input);
        return bytes;
    }

    public static byte[] longArrayToByteArray(long[] input) {
        return floatArrayToByteArray(longArrayToFloatArray(input));
    }

    public static float[] longArrayToFloatArray(long[] input) {
        float[] floats = new float[input.length];
        for (int i = 0; i < input.length; i++) {
            floats[i] = input[i];
        }
        return floats;
    }

    public static void main(String[] args) {
        // Connect to Redis
        UnifiedJedis unifiedjedis = new UnifiedJedis(System.getenv().getOrDefault("REDIS_URL", "redis://localhost:6379"));

        // Create the index
        IndexDefinition definition = new IndexDefinition().setPrefixes(new String[]{"doc:"});
        Map<String, Object> attr = new HashMap<>();
        attr.put("TYPE", "FLOAT32");
        attr.put("DIM", 768);
        attr.put("DISTANCE_METRIC", "L2");
        attr.put("INITIAL_CAP", 3);
        Schema schema = new Schema().addTextField("content", 1).addTagField("genre").addHNSWVectorField("embedding", attr);                      
        unifiedjedis.ftCreate("vector_idx", IndexOptions.defaultOptions().setDefinition(definition), schema);

        // Create the embedding model
        Map<String, String> options = Map.of("maxLength", "768",  "modelMaxLength", "768");
        HuggingFaceTokenizer sentenceTokenizer = HuggingFaceTokenizer.newInstance("sentence-transformers/all-mpnet-base-v2", options);
        
        // Train with sentences
        String sentence1 = "That is a very happy person";
        unifiedjedis.hset("doc:1", Map.of(  "content", sentence1, "genre", "persons"));
        unifiedjedis.hset("doc:1".getBytes(), "embedding".getBytes(), longArrayToByteArray(sentenceTokenizer.encode(sentence1).getIds()));
        
        String sentence2 = "That is a happy dog";
        unifiedjedis.hset("doc:2", Map.of(  "content", sentence2, "genre", "pets"));
        unifiedjedis.hset("doc:2".getBytes(), "embedding".getBytes(), longArrayToByteArray(sentenceTokenizer.encode(sentence2).getIds()));

        String sentence3 = "Today is a sunny day";
        Map<String, String> doc3 = Map.of(  "content", sentence3, "genre", "weather");
        unifiedjedis.hset("doc:3", doc3);
        unifiedjedis.hset("doc:3".getBytes(), "embedding".getBytes(), longArrayToByteArray(sentenceTokenizer.encode(sentence3).getIds()));

        // This is the test sentence
        String sentence = "That is a happy person";

        int K = 3;
        Query q = new Query("*=>[KNN $K @embedding $BLOB AS score]").
                            returnFields("content", "score").
                            addParam("K", K).
                            addParam("BLOB", longArrayToByteArray(sentenceTokenizer.encode(sentence).getIds())).
                            dialect(2);

        // Execute the query
        List<Document> docs = unifiedjedis.ftSearch("vector_idx", q).getDocuments();
        System.out.println(docs);
    }
}
