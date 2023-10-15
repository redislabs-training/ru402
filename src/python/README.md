
# RU402 RedisVL (Python) Example

The following example demonstrates the execution of a simple example to model sentences as vector embeddings using the [RedisVL](https://github.com/RedisVentures/redisvl/) client for the Python programming language. Along the course we have used the well-known [redis-py](https://redis-py.readthedocs.io/en/stable/). RedisVL is built on top of redis-py and abstracts several operations, such as index management, creation of vector embeddings and vector search, and semantic caching. Refer to the [repository](https://github.com/RedisVentures/redisvl/) to learn more.

## Prerequisites

* You'll need [RedisVL](https://github.com/RedisVentures/redisvl/) installed. You can install it on your machine with `pip install redisvl`
* You will need an instance of Redis Stack.  See the [setup instructions](/README.md) in the README at the root of this repo.
* If you are running your Redis Stack instance in the cloud or somewhere that isn't localhost:6379, you'll need to set the `REDIS_URL` environment variable to point at your instance before running the code.  If you need help with the format for this, check out the [Redis URI scheme specification](https://www.iana.org/assignments/uri-schemes/prov/redis).

## Run the Code

Ensure that your Redis Stack instance is running, and that you have set the `REDIS_URL` environment variable if necessary.  Example:

```bash
export REDIS_URL=redis://user:password@host:port
```

> By default, the connection is attemped to a localhost Redis Stack instance on port `6379`

You can now install the libraries required to run the example. You can create a Python virtual environment as usual.

```
python3 -m venv vssvenv
source vssvenv/bin/activate

pip install redisvl
pip install sentence_transformers
```

Now you can run the example with:

```
python App.py
```

The example will store three sentences and test one sentence for similarity. The result is:

```text
[{'id': 'doc:e993b504b5254f31b809f3ba8c65e31d', 'vector_distance': '0.114169858396', 'content': 'That is a very happy person'}, {'id': 'doc:2f6714bf5f3643b6b73b0c7f8f146b48', 'vector_distance': '0.610845565796', 'content': 'That is a happy dog'}, {'id': 'doc:c993c84f60c04c6d91908c09dfc3036d', 'vector_distance': '1.48624742031', 'content': 'Today is a sunny day'}]
```

## View the Code

The code is contained in the file [`App.py`](./App.py).

## References

- [RedisVL](https://github.com/RedisVentures/redisvl/) experimental client for the Python programming language
- [redis-py](https://redis-py.readthedocs.io/en/stable/) supported client for the Python programming language
- [https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2"](all-MiniLM-L6-v2) the embedding model utilized in the example
