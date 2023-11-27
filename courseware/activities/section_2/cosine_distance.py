import numpy as np
from numpy.linalg import norm
from sentence_transformers import SentenceTransformer

# Define the model we want to use (it'll download itself)
model = SentenceTransformer('sentence-transformers/all-MiniLM-L6-v2')

sentences = [
"That is a very happy person", 
"That is a happy dog",
"Today is a sunny day"
]

sentence = "That is a happy person"

# vector embeddings created from dataset
embeddings = model.encode(sentences)

# query vector embedding
query_embedding = model.encode(sentence)

# define our distance metric
def cosine_similarity(a, b):
    return np.dot(a, b)/(norm(a)*norm(b))

# run semantic similarity search
print("Query: That is a happy person") 
for e, s in zip(embeddings, sentences):
    print(s, " -> similarity score = ", cosine_similarity(e, query_embedding))