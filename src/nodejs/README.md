# RU402 node-redis (Node.js) Example

The following example demonstrates the execution of a simple example to model sentences as vector embeddings using the [node-redis](https://github.com/redis/node-redis) client for the JavaScript programming language for Node.js.

## Prerequisites

* You'll need [Node.js](https://nodejs.org/) or higher installed. You can install it on your machine with `install npm`
* You will need an instance of Redis Stack.  See the [setup instructions](/README.md) in the README at the root of this repo.
* If you are running your Redis Stack instance in the cloud or somewhere that isn't localhost:6379, you'll need to set the `REDIS_URL` environment variable to point at your instance before running the code.  If you need help with the format for this, check out the [Redis URI scheme specification](https://www.iana.org/assignments/uri-schemes/prov/redis).

## Run the Code

Ensure that your Redis Stack instance is running, and that you have set the `REDIS_URL` environment variable if necessary.  Example:

```bash
export REDIS_URL=redis://user:password@host:port
```

> By default, the connection is attemped to a localhost Redis Stack instance on port `6379`

You can now install the modules required to run the example. From the folder where the source `App.js` is located, run:

```
npm install redis
npm install @xenova/transformers
```

To complete the setup, declare the project as a module by editing the `package.json` file, which should be like:

```
{
  "type": "module",
  "dependencies": {
    "@xenova/transformers": "^2.6.2",
    "redis": "^4.6.10"
  }
}
```

Now you can run the example with:

```
node App.js 
```

The example will store three sentences and test one sentence for similarity. The result is:

```text
Vector search results found: 3.
doc:1: That is a very happy person with score 0.127055495977
doc:2: That is a happy dog with score 0.836842417717
doc:3: Today is a sunny day with score 1.50889515877
```

## View the Code

The code is contained in the file [`App.js`](./App.js).