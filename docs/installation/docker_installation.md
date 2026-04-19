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
      - scriptbee-plugins:/app/plugins
      - /var/run/docker.sock:/var/run/docker.sock

    environment:
      - ConnectionStrings__mongodb=mongodb://root:example@mongo:27017/ScriptBee?authSource=admin
      - UserFolder__UserFolderPath=/root/.scriptbee
      - SCRIPTBEE__ANALYSIS__DRIVER=docker
      - SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET=unix:///var/run/docker.sock
      - SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH=${PWD}/database/scriptbee
      - SCRIPTBEE__PLUGINS__INSTALLATIONFOLDER=/app/plugins
    depends_on:
      - mongo

volumes:
  scriptbee-plugins:
  mongodb_data:
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

See [Analysis Configuration](../architecture/configuration/gateway_configuration.md#analysis-configuration) for full configuration variables

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
