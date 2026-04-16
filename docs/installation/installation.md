# Installation

## Prerequisites

- Mongo 8.0.4
- Dotnet 10.0
- Node 24.0

## Configuration

ScriptBee is configured primarily through environment variables. The most critical settings relate to how it manages
analysis instances.

See [Configuration](../architecture/configuration/gateway_configuration.md) page for full configuration

### MongoDB

ScriptBee uses MongoDB as a database. In order to run ScriptBee, you need to set up a MongoDB server and set the
connection string as an environment variable.

For example:

```yaml
environment:
  - ConnectionStrings__mongodb=mongodb://mongo:27017/ScriptBee?authSource=admin
```

### ScriptBee Data

This is the path to the folder where the ScriptBee data will be stored. This includes the scripts and plugins. You can
see in the following example how the folder structure looks like

```
./scriptbee_data

└── plugins
    ├── plugin1
    └── plugin2
└── projects
    └── project1
      ├── .generated
      └── src
        └── script.cs
```

### User Folder Setup

> [!CAUTION]
> This setup is obsolete and will be replaced by editing either via UI or via the VS Code Extension

In order to open the scripts in Visual Studio Code, you need to set an environment variable to the user folder in the
following environment variable: `UserFolder__UserFolderPath`.

This should be the absolute path of the host machine to the folder where the ScriptBee data is stored.

## Driver Configuration

### Docker Compose

```dotenv
SCRIPTBEE__ANALYSIS__DRIVER=docker
```

See [Docker Compose Installation](docker_installation.md) for more information

### Kubernetes

```dotenv
SCRIPTBEE__ANALYSIS__DRIVER=kubernetes
```

See [Kubernetes Installation](kubernetes_installation.md) for more information
