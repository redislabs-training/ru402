
# RU402 .NET (C#) Example

The following example demonstrates the execution of a simple example to model sentences as vector embeddings using the [NredisStack](https://github.com/redis/NRedisStack/) client for the .NET programming language.

## Prerequisites

* The example has been tested using .NET 7.0
* You will need an instance of Redis Stack.  See the [setup instructions](/README.md) in the README at the root of this repo.
* If you are running your Redis Stack instance in the cloud or somewhere that isn't localhost:6379, you'll need to set the `REDIS_URL` environment variable to point at your instance before running the code.  If you need help with the format for this, check out the [Redis URI scheme specification](https://www.iana.org/assignments/uri-schemes/prov/redis).

## Run the Code

Ensure that your Redis Stack instance is running. You can execute the project with:

```
dotnet run
```

The example will store three sentences and test one sentence for similarity. The result is:

```text
id: doc:1,  4.30777168274 That is a very happy person
id: doc:2,  25.9752807617 That is a happy dog
id: doc:3,  68.8638000488 Today is a sunny day
```

### Create a new project

The example is ready for testing. However, if you'd like to recreate a new project from scratch, remember to install the following libraries.

```
dotnet new console -o my-app -f net7.0

dotnet add package NRedisStack
dotnet add package Microsoft.ML
```

## View the Code

The code is contained in [`Program.cs`](./my-app/Program.cs).


## References

- [TextCatalog.ApplyWordEmbedding](https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.textcatalog.applywordembedding?view=ml-dotnet), the .NET text featurizer that converts a vector of text into a numerical vector using pre-trained embeddings models
- [WordEmbeddingEstimator.PretrainedModelKind](https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.transforms.text.wordembeddingestimator.pretrainedmodelkind?view=ml-dotnet), the list of pre-trained models.
- [C#/.NET guide](https://redis.io/docs/clients/dotnet/), a quickstart guide to work with NredisStack
- [NredisStack](https://github.com/redis/NRedisStack/), the Redis client for the .NET programming language















