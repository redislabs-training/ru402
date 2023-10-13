# RU402 Jedis (Java) Example

The following example demonstrates the execution of a simple example to model sentences as vector embeddings using the [Jedis](https://github.com/redis/jedis) client for the Java programming language.

## Prerequisites

* You'll need [Java 11](https://sdkman.io/sdks) or higher installed.
* You will need an instance of Redis Stack.  See the [setup instructions](/README.md) in the README at the root of this repo.
* If you are running your Redis Stack instance in the cloud or somewhere that isn't localhost:6379, you'll need to set the `REDIS_URL` environment variable to point at your instance before running the code.  If you need help with the format for this, check out the [Redis URI scheme specification](https://www.iana.org/assignments/uri-schemes/prov/redis).

## Run the Code

Ensure that your Redis Stack instance is running, and that you have set the `REDIS_URL` environment variable if necessary.  Example:

```bash
export REDIS_URL=redis://user:password@host:port
```

The example is provided as a Maven project, which you can compile using

```bash
mvn package
```

And execute using:

```bash
mvn exec:java -Dexec.mainClass=com.redis.app.App
```

When the execution completes, the following output is printed:

```text
[id:doc:1, score: 1.0, properties:[score=9301635, content=That is a very happy person], id:doc:2, score: 1.0, properties:[score=1411344, content=That is a happy dog], id:doc:3, score: 1.0, properties:[score=67178800, content=Today is a sunny day]]
```

## View the Code

The code is contained in [`App.java`](./my-app/src/main/java/com/redis/app/App.java).