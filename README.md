# ScriptBee

<p align="center">
  <img src="docs/public/assets/logo.png" alt="ScriptBee Logo" width="200"/>
</p>

## What is ScriptBee?

ScriptBee is a tool that helps the analysis of different models. Different tools create data and with the help of
loaders, ScriptBee can load the data and create a model. The model can then be analyzed by running different scripts
written in C#, Javascript and Python on it.

## Quick Start

The fastest way to get started with ScriptBee is using Docker Compose.

### Run with Docker

To get ScriptBee running quickly, create a `docker-compose.yaml` file with the following content and run
`docker compose up -d`:

```yaml
version: "3.8"
services:
  mongo:
    image: mongo:4.4
    container_name: mongo
    restart: unless-stopped
    volumes:
      - mongodb_data:/data/db

  scriptbee:
    image: dxworks/scriptbee
    ports:
      - "4201:80"
    volumes:
      - scriptbee_data:/root/.scriptbee
    environment:
      - ConnectionStrings__mongodb=mongodb://mongo:27017/ScriptBee?authSource=admin
      - UserFolder__UserFolderPath=/root/.scriptbee
    depends_on:
      - mongo

volumes:
  mongodb_data:
  scriptbee_data:
```

### Accessing the UI

Once the containers are running, open your browser and navigate to **[http://localhost:4201](http://localhost:4201)**.

---

## Configuration

ScriptBee can be configured using environment variables. Below are the most important settings:

| Variable                     | Description                                                      | Default                    |
|------------------------------|------------------------------------------------------------------|----------------------------|
| `ConnectionStrings__mongodb` | Connection string for the MongoDB instance used for persistence. | `mongodb://mongo:27017...` |
| `UserFolder__UserFolderPath` | Host path for storing project data and shared files.             |                            |
| `SCRIPTBEE_ANALYSIS__DRIVER` | How analysis instances are managed: `docker` or `kubernetes`.    | `docker`                   |

For more detailed information on advanced configuration, check the *
*[Deployment Features](docs/architecture/features.md)** documentation.

---

## Documentation

Full documentation is available at [https://dxworks.org/scriptbee/](https://dxworks.org/scriptbee/).

## Contributing

For information on the repository structure, development setup, and how to contribute, please refer to our *
*[Contributing Guide](.github/CONTRIBUTING.md)**.
