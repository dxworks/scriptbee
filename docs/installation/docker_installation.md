# Docker Compose

You start ScriptBee using Docker. In order to do so, you need to setup a MongoDB server and set the connection string as
an environment variable.

An example of the `docker-compose.yaml`:

```yaml title="docker-compose.yaml"
services:
  mongo:
    image: mongo:8.0.4
    container_name: mongo
    restart: unless-stopped
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: example
    ports:
      - '27017:27017'
    volumes:
      - ./database:/data/db

  scriptbee:
    image: dxworks/scriptbee:latest
    user: root
    ports:
      - '4201:80'
    volumes:
      - ./database/scriptbee:/root/.scriptbee
      - /var/run/docker.sock:/var/run/docker.sock

    environment:
      - ConnectionStrings__mongodb=mongodb://root:example@mongo:27017/ScriptBee?authSource=admin
      - UserFolder__UserFolderPath=/root/.scriptbee
      - SCRIPTBEE__ANALYSIS__DRIVER=docker
      - SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET=unix:///var/run/docker.sock
      - SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH=${PWD}/database/scriptbee
    depends_on:
      - mongo
```

## How to run

To run ScriptBee simply run the following command:

```shell
docker-compose up
```

## Driver Configuration

To enable Docker-managed analysis, set:

```dotenv
SCRIPTBEE__ANALYSIS__DRIVER=docker
```

Once enabled, the Gateway will spawn analysis instances as standalone Docker containers.

## Minimum Configuration

For a basic setup, the minimum required environment variables are:

```yaml
environment:
  - SCRIPTBEE__ANALYSIS__DRIVER=docker
  - SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET=unix:///var/run/docker.sock
  - SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH=${PWD}/database/scriptbee
```

### Full Configuration Reference

You can fine-tune the Docker driver behavior using the following environment variables. In .NET, these are mapped from
the configuration section `ScriptBee:Analysis:Docker`.

| Environment Variable                                   | Description                                                                                       | Default / Example                   |
| ------------------------------------------------------ | ------------------------------------------------------------------------------------------------- | ----------------------------------- |
| `SCRIPTBEE__ANALYSIS__IMAGE`                           | The Docker image used for analysis instances.                                                     | `dxworks/scriptbee-analysis:latest` |
| `SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET`            | URI to the Docker daemon socket.                                                                  | `unix:///var/run/docker.sock`       |
| `SCRIPTBEE__ANALYSIS__DOCKER__PORT`                    | The internal port the analysis container listens on.                                              | `80`                                |
| `SCRIPTBEE__ANALYSIS__DOCKER__NETWORK`                 | The Docker network name to attach containers to (required if using a custom network for MongoDB). | _None_                              |
| `SCRIPTBEE__ANALYSIS__DOCKER__MONGODBCONNECTIONSTRING` | MongoDB connection string for the analysis instance.                                              | _Inherits Gateway's if omitted_     |
| `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERVOLUMEPATH`    | The mount point for project data _inside_ the analysis container.                                 | `/root/.scriptbee`                  |
| `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH`      | THE absolute path on the **host machine** where project data is stored.                           | `${PWD}/database/scriptbee`         |

### Minimum Configuration

For a basic setup (e.g., using the default Docker Compose), the minimum environment variables required are:

```yaml
environment:
  - SCRIPTBEE__ANALYSIS__DRIVER=docker
  - SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET=unix:///var/run/docker.sock # or npipe://./pipe/docker_engine on Windows
  - SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH=${PWD}/database/scriptbee # Absolute path to SCRIPTBEE_DATA on host
```

### Full Configuration Reference

You can fine-tune the Docker driver behavior using the following environment variables. In .NET, these are mapped from
the configuration section `ScriptBee:Analysis:Docker`.

| Environment Variable                                   | Description                                                                                       | Default / Example                   |
| ------------------------------------------------------ | ------------------------------------------------------------------------------------------------- | ----------------------------------- |
| `SCRIPTBEE__ANALYSIS__IMAGE`                           | The Docker image used for analysis instances.                                                     | `dxworks/scriptbee-analysis:latest` |
| `SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET`            | URI to the Docker daemon socket.                                                                  | `unix:///var/run/docker.sock`       |
| `SCRIPTBEE__ANALYSIS__DOCKER__PORT`                    | The internal port the analysis container listens on.                                              | `80`                                |
| `SCRIPTBEE__ANALYSIS__DOCKER__NETWORK`                 | The Docker network name to attach containers to (required if using a custom network for MongoDB). | _None_                              |
| `SCRIPTBEE__ANALYSIS__DOCKER__MONGODBCONNECTIONSTRING` | MongoDB connection string for the analysis instance.                                              | _Inherits Gateway's if omitted_     |
| `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERVOLUMEPATH`    | The mount point for project data _inside_ the analysis container.                                 | `/root/.scriptbee`                  |
| `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH`      | The absolute path on the **host machine** where project data is stored.                           | _Inherits Gateway's if omitted_     |

> [!IMPORTANT]
> The `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH` must be an **absolute path** on your host machine. This is
> because the Gateway tells the Docker daemon to mount this path into the new analysis containers. If this is incorrect,
> the analysis service will not be able to find your scripts or models.

## ScriptBee Data

ScriptBee stores the data in the user folder, in this case `/root/.scriptbee`.
In order to access the data from the host machine, you need to mount a volume to the container.

For example:

```yaml
volumes:
  - ./scriptbee_data:/root/.scriptbee
```

## User Folder Setup

This should be the absolute path of the host machine to the folder where the ScriptBee data is stored.

```yaml
scriptbee:
  image: dxworks/scriptbee:latest
  user: root

  volumes:
    - ./database/scriptbee:/root/.scriptbee

  environment:
    - UserFolder__UserFolderPath=/root/.scriptbee
```

## Docker Hosting Tips

When hosting ScriptBee with Docker, pay attention to the following settings:

- **`user: root`**: Required for the main container to manage other containers (Analysis instances).
- **`${PWD}`**: Always use `${PWD}` or an absolute path for `USERFOLDERHOSTPATH`. Docker requires absolute paths when
  creating containers from within another container.
- **`unix:///var/run/docker.sock`**: Standard socket for Docker communication.
