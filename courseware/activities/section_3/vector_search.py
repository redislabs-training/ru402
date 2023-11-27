import redis
from redis.commands.search.query import Query
import numpy as np
from numpy.linalg import norm
from sentence_transformers import SentenceTransformer


# Get a Redis connection
r = redis.Redis(host='localhost', port=6379)

# Define the model we want to use 
model = SentenceTransformer('sentence-transformers/all-MiniLM-L6-v2')

# Import sample data
r.hset('doc:1', mapping = {'embedding': model.encode("That is a very happy person").astype(np.float32).tobytes(), 'genre': 'persons', 'content': "That is a very happy person"})
r.hset('doc:2', mapping = {'embedding': model.encode("That is a happy dog").astype(np.float32).tobytes(), 'genre': 'pets', 'content': "That is a happy dog"})
r.hset('doc:3', mapping = {'embedding': model.encode("Today is a sunny day").astype(np.float32).tobytes(), 'genre': 'weather', 'content': "Today is a sunny day"})

# This is the test sentence
sentence = "That is a happy person"

q = Query("*=>[KNN 2 @embedding $vec AS score]").return_field("score").return_field("content").dialect(2)
res = r.ft("doc_idx").search(q, query_params={"vec": model.encode(sentence).astype(np.float32).tobytes()})
print(res)