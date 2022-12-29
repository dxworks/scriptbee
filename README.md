# ScriptBee

## What is ScriptBee?

ScriptBee is a tool that helps the analysis of different models. Different tools create data and with the help of
loaders, ScriptBee can load the data and create a model. The model can then be analyzed by running different scripts
written in C#, Javascript and Python on it.

## How to run

### Setup

Create a `.env` next to `docker-compose.yaml` in order to setup the local environment.
Or change the variables directly in the `docker-compose.yaml`.

The following variables need to be set:

- MONGODB_DATA
- SCRIPTBEE_DATA

#### MONGODB_DATA

This is the path to the folder where the MongoDB data will be stored.

#### SCRIPTBEE_DATA

This is the path to the folder where the ScriptBee data will be stored. This includes the scripts and plugins

```
SCRIPTBEE_DATA
└── plugins
    ├── plugin1
    └── plugin2
└── projects
    └── project1
      ├── generated
      └── src
        └── script.cs
```

### Run

To run ScriptBee simply run the following command:

```bash
docker-compose up
```

> For local development, run the following command: `docker-compose -f docker-compose-dev.yaml up`

## Plugins

> Plugins use the [DxWorks Hub SDK](https://github.com/dxworks/dxworks-hub-sdk) and will create a folder in the users
> folder were the hub will be downloaded and used. (e.g. `C:\Users\{user}\.dxw\hub`)

### How to create a plugin

To create a plugin you need to create a folder in the `plugins` folder in the folder `SCRIPTBEE_DATA`. The folder must
contain a `manifest.yaml` file where the definition of the plugin is described.

ScriptBee supports the following plugin types:

- Loader
- Linker
- ScriptGenerator
- ScriptRunner
- HelperFunctions

[//]: # (- UI)

[//]: # (> Note: the `UI` plugin type is not yet fully implemented)

Some examples of a plugin manifests are located [here](ScriptBee.Tests/Plugin/Manifest/TestData).

### Anatomy of the plugin manifest

```yaml
apiVersion: 1.0.0
author: ScriptBee
description: "Description"
name: "HelperFunctions example"
extensionPoints:
  - kind: HelperFunctions
    entryPoint: Plugin.dll
    version: "0.0.1"
```

- `apiVersion`: The version of the plugin manifest
- `author`: The author of the plugin
- `description`: A description of the plugin
- `name`: The name of the plugin
- `extensionPoints`: A list of extension points that the plugin provides
- `kind`: The type of plugin
- `entryPoint`: The entry point of the plugin, usually the dll containing the implemented interfaces for the respective
  plugins
- `version`: The version of the plugin

> Note: depending on the plugin type, each extension point can have different properties

### How to install a plugin

To install a plugin, simply copy the plugin folder to the `SCRIPTBEE_DATA` folder. The plugin will be automatically
loaded when ScriptBee is started.
The plugin folder must be located in the `plugins` folder and must contain the `manifest.yaml` file.

##### Example

```
SCRIPTBEE_DATA
└── plugins
    └── HelperFunctionsExample
        ├── manifest.yaml
        └── Plugin.dll
```
