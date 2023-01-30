# Installation

## Docker Compose

You start ScriptBee using Docker. In order to do so, you need to setup a MongoDB server and set the connection string as
an environment variable.

An example of the `docker-compose.yaml`:

```yaml title="docker-compose.yaml"
version: "3.8"
services:
  mongo:
    image: mongo:latest
    container_name: mongo
    restart: unless-stopped
    volumes:
      - ./database:/data/db

  scripbee:
    image: dxworks/scriptbee
    ports:
      - "4201:80"
    volumes:
      - /host/scriptbee_data:/root/.scriptbee

    environment:
      - UserFolder__UserFolderPath=/host/scriptbee_data
      - ConnectionStrings__mongodb=mongodb://mongo:27017/ScriptBee?authSource=admin
    depends_on:
      - mongo
```

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
