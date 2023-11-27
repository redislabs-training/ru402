import requests
from PIL import Image
from imgbeddings import imgbeddings


url = "http://images.cocodataset.org/val2017/000000039769.jpg"
image = Image.open(requests.get(url, stream=True).raw)
ibed = imgbeddings()
embedding = ibed.to_embeddings(image)
print(embedding[0][0:5])