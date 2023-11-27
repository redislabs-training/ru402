from sentence_transformers import SentenceTransformer


model = SentenceTransformer('sentence-transformers/all-MiniLM-L6-v2')
text = "This is a technical document, it describes the SID sound chip of the Commodore 64" 
embedding = model.encode(text)

print(embedding[:10])