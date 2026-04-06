# Installation

## Prerequisites

- Mongo 4.4
- Dotnet 6.0
- Node 16.0

## Docker Compose

You start ScriptBee using Docker. In order to do so, you need to setup a MongoDB server and set the connection string as
an environment variable.

An example of the `docker-compose.yaml`:

```yaml title="docker-compose.yaml"
version: '3.8'
services:
  mongo:
    image: mongo:4.4
    container_name: mongo
    restart: unless-stopped
    volumes:
      - ./database:/data/db

  scripbee:
    image: dxworks/scriptbee
    ports:
      - '4201:80'
    volumes:
      - /host/scriptbee_data:/root/.scriptbee

    environment:
      - UserFolder__UserFolderPath=/host/scriptbee_data
      - ConnectionStrings__mongodb=mongodb://mongo:27017/ScriptBee?authSource=admin
    depends_on:
      - mongo
```

#### How to run

To run ScriptBee simply run the following command:

```shell
docker-compose up
```

### Driver Configuration

The Analysis service can be configured to use different drivers for managing analysis instances. Currently, the primary driver is **Docker**.

#### Docker Driver Configuration

To use the Docker driver, set the following environment variable:

```dotenv
SCRIPTBEE__ANALYSIS__DRIVER=docker
```

Once enabled, the Gateway will spawn analysis instances as standalone Docker containers.

##### Minimum Configuration

For a basic setup (e.g., using the default Docker Compose), the minimum environment variables required are:

```yaml
environment:
  - SCRIPTBEE__ANALYSIS__DRIVER=docker
  - SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET=unix:///var/run/docker.sock # or npipe://./pipe/docker_engine on Windows
  - SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH=/path/to/your/data # Absolute path to SCRIPTBEE_DATA on host
```

##### Full Configuration Reference

You can fine-tune the Docker driver behavior using the following environment variables. In .NET, these are mapped from the configuration section `ScriptBee:Analysis:Docker`.

| Environment Variable                                   | Description                                                                                       | Default / Example                   |
| ------------------------------------------------------ | ------------------------------------------------------------------------------------------------- | ----------------------------------- |
| `SCRIPTBEE__ANALYSIS__IMAGE`                           | The Docker image used for analysis instances.                                                     | `dxworks/scriptbee/analysis:latest` |
| `SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET`            | URI to the Docker daemon socket.                                                                  | `unix:///var/run/docker.sock`       |
| `SCRIPTBEE__ANALYSIS__DOCKER__PORT`                    | The internal port the analysis container listens on.                                              | `8080`                              |
| `SCRIPTBEE__ANALYSIS__DOCKER__NETWORK`                 | The Docker network name to attach containers to (required if using a custom network for MongoDB). | _None_                              |
| `SCRIPTBEE__ANALYSIS__DOCKER__MONGODBCONNECTIONSTRING` | MongoDB connection string for the analysis instance.                                              | _Inherits Gateway's if omitted_     |
| `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERVOLUMEPATH`    | The mount point for project data _inside_ the analysis container.                                 | `/root/.scriptbee`                  |
| `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH`      | The absolute path on the **host machine** where project data is stored.                           | _Inherits Gateway's if omitted_     |

> [!IMPORTANT]
> The `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH` must be an **absolute path** on your host machine. This is because the Gateway tells the Docker daemon to mount this path into the new analysis containers. If this is incorrect, the analysis service will not be able to find your scripts or models.

### MongoDB

ScriptBee uses MongoDB as a database. In order to run ScriptBee, you need to setup a MongoDB server and set the
connection string as an environment variable.

For example:

```yaml
environment:
  - ConnectionStrings__mongodb=mongodb://mongo:27017/ScriptBee?authSource=admin
```

### ScriptBee Data

ScriptBee stores the data in the user folder, in this case `/root/.scriptbee`.
In order to access the data from the host machine, you need to mount a volume to the container.

For example:

```yaml
volumes:
  - ./scriptbee_data:/root/.scriptbee
```

This is the path to the folder where the ScriptBee data will be stored. This includes the scripts and plugins. You can
see in the following example how the folder structure looks like

```
./scriptbee_data

└── plugins
    ├── plugin1
    └── plugin2
└── projects
    └── project1
      ├── generated
      └── src
        └── script.cs
```

### User Folder Setup

In order to open the scripts in Visual Studio Code, you need to set an environment variable to the user folder in the
following environment variable: `UserFolder__UserFolderPath`.

This should be the absolute path of the host machine to the folder where the ScriptBee data is stored.

```yaml
scripbee:
  image: dxworks/scriptbee

  volumes:
    - ./scriptbee_data:/root/.scriptbee

  environment:
    - UserFolder__UserFolderPath=/root/scriptbee_data
```

## Kubernetes

### Driver Configuration

> [!CAUTION]
> Still work in progress

For the Kubernetes deployment, the following environment variables need to be set

```dotenv
SCRIPTBEE__ANALYSIS__DRIVER=kubernetes
```
