# ScriptBee

## What is ScriptBee?

ScriptBee is a tool that helps the analysis of different models. Different tools create data and with the help of
loaders, ScriptBee can load the data and create a model. The model can then be analyzed by running different scripts
written in C#, Javascript and Python on it.

## Documentation

The full documentation can be found [here](https://dxworks.org/scriptbee/).

### Run locally MkDocs

```bash
docker run --rm -it -p 8000:8000 -v ${PWD}:/docs squidfunk/mkdocs-material
```

## How to run

### Run

To run ScriptBee simply run the following command:

```bash
docker-compose up
```

For local development, run the following command:

```sh 
docker-compose -f docker-compose-dev.yaml up
```
