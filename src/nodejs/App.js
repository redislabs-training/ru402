import {VectorAlgorithms, createClient, SchemaFieldTypes} from 'redis';
import * as transformers from '@xenova/transformers';


const REDIS_URL =  process.env['REDIS_URL'] || 'redis://localhost:6379';
let nodeRedisClient = null;


const getNodeRedisClient = async () => {
  if (!nodeRedisClient) {
    nodeRedisClient = createClient({ url: REDIS_URL });
    await nodeRedisClient.connect();
  }
  return nodeRedisClient;
};


async function createIndex() {
    const schema = {
        'content': {
            type: SchemaFieldTypes.TEXT,
            sortable: false
        },
        'genre': {
            type: SchemaFieldTypes.TAG,
            sortable: false
        },
        'embedding': {
            type: SchemaFieldTypes.VECTOR,
            TYPE: 'FLOAT32',
            ALGORITHM: VectorAlgorithms.HNSW,
            DIM: 768,
            DISTANCE_METRIC: 'L2',
            INITIAL_CAP: 3,
            AS: 'embedding',
        }
    }

    try {
        const client = await getNodeRedisClient();
        await client.ft.create('vector_idx', schema, {
        ON: 'HASH',
        PREFIX: 'doc:'
        });
    }
    catch (e) {
        if (e.message === 'Index already exists') {
            console.log('Index exists already, skipped creation.');
        } else {
            console.error(e);
            process.exit(1);
        }
    }
}


const float32Buffer = (arr) => {
    const floatArray = new Float32Array(arr);
    const float32Buffer = Buffer.from(floatArray.buffer);
    return float32Buffer;
};


async function generateSentenceEmbeddings(sentence) {
    let modelName = 'Xenova/all-distilroberta-v1';
    let pipe = await transformers.pipeline('feature-extraction', modelName);

    let vectorOutput = await pipe(sentence, {
        pooling: 'mean',
        normalize: true,
    });

    const embedding = Object.values(vectorOutput?.data);
    return embedding;
}


async function createEmbeddings() {
    const client = await getNodeRedisClient();
    
    const sentence1 = {   "content":"That is a very happy person", 
                    "genre":"persons", 
                    "embedding":float32Buffer(await generateSentenceEmbeddings("That is a very happy person"))}

    const sentence2 = {   "content":"That is a happy dog", 
                    "genre":"pets", 
                    "embedding":float32Buffer(await generateSentenceEmbeddings("That is a happy dog"))}

    const sentence3 = {   "content":"Today is a sunny day", 
                    "genre":"weather", 
                    "embedding":float32Buffer(await generateSentenceEmbeddings("Today is a sunny day"))}
    
    await Promise.all([
        client.hSet('doc:1', sentence1),
        client.hSet('doc:2', sentence2),
        client.hSet('doc:3', sentence3)
    ]);
}


async function vss() {
    const client = await getNodeRedisClient();

    const similar = await client.ft.search(
        'vector_idx',
        '*=>[KNN 3 @embedding $B AS score]',{
            "PARAMS": 
                {
                    B: float32Buffer(await generateSentenceEmbeddings("That is a happy person")),
                },
            "RETURN": ['score', 'content'],
            "DIALECT": "2"
        },
   );
   
   console.log(`VSS results found: ${similar.total}.`);
   for (const doc of similar.documents) {
       console.log(`${doc.id}: ${doc.value.content} with score ${doc.value.score}`);
   }
}


await createIndex();
await createEmbeddings();
await vss();
