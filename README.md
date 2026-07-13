# ScriptBee

<p align="center">
  <img src="docs/public/assets/logo.svg" alt="ScriptBee Logo" width="200"/>
</p>

## What is ScriptBee?

ScriptBee is a tool that helps the analysis of different models. Different tools create data and with the help of
loaders, ScriptBee can load the data and create a model. The model can then be analyzed by running different scripts
written in C#, Javascript and Python on it.

## Documentation

Full documentation is available at [https://dxworks.org/scriptbee/](https://dxworks.org/scriptbee/).

## Quick Start

The only prerequisite is [Docker Desktop](https://docs.docker.com/get-docker/).

Open a terminal in any folder and run the command for your OS — it handles everything else.

**Linux / macOS:**

```bash
curl -fsSL https://raw.githubusercontent.com/dxworks/scriptbee/main/quickstart/start.sh | bash
```

**Windows (PowerShell):**

```powershell
irm https://raw.githubusercontent.com/dxworks/scriptbee/main/quickstart/start.ps1 | iex
```

The script will:

1. Create a `scriptbee/` folder in your current directory
2. Start ScriptBee and MongoDB via Docker using Docker Compose

Once running, open your browser and navigate to **[http://localhost:4201](http://localhost:4201)**.

To stop all services:

```bash
docker compose -f scriptbee/docker-compose.yaml down
```

> **Already cloned the repo?** Run `bash quickstart/start.sh` (or `.\quickstart\start.ps1`) directly from the repo root

---

## Configuration

ScriptBee can be configured using environment variables. Below are the most important settings:

| Variable                     | Description                                                      | Default                    |
|------------------------------|------------------------------------------------------------------|----------------------------|
| `ConnectionStrings__mongodb` | Connection string for the MongoDB instance used for persistence. | `mongodb://mongo:27017...` |
| `UserFolder__UserFolderPath` | Host path for storing project data and shared files.             |                            |
| `SCRIPTBEE_ANALYSIS__DRIVER` | How analysis instances are managed: `docker` or `kubernetes`.    | `docker`                   |

Check the [Configuration Reference](https://dxworks.org/scriptbee/architecture/configuration/gateway_configuration.html)
for a complete list of configuration options.

---

## VS Code Extension

The official **ScriptBee VS Code Extension** lets you manage your scripts and connections directly
from your editor. Key capabilities include:

- 🔌 **Multi-connection management** — connect to local, staging, or production ScriptBee instances.
- 📂 **Script synchronization** — push, pull, or sync all scripts; or push/pull a single file via right-click.
- 🔍 **Compare with Remote** — built-in diff view to review server changes before syncing.

### Install

Search for **ScriptBee** in the VS Code Extensions Marketplace, or install from VSIX by downloading
from the [GitHub Releases](https://github.com/dxworks/scriptbee/releases) page.

For full extension documentation, including configuration options and a complete command reference, see the
[VS Code Extension Guide](https://dxworks.org/scriptbee/installation/vs_code_extension).

---

## MCP Server

ScriptBee ships an official **Model Context Protocol (MCP) server** that exposes its capabilities — project management,
context loading, script execution, and analysis — to AI clients such as Claude Desktop, Claude Code, GitHub Copilot, and
VS Code.

The server is available on Docker Hub: [**dxworks/scriptbee-mcp**](https://hub.docker.com/r/dxworks/scriptbee-mcp).

### Configure in VS Code

Add the MCP server to your `.vscode/mcp.json` using one of the two supported transport modes.

**HTTP transport** — connect to an already-running MCP server:

```json
{
  "mcpServers": {
    "scriptbee": {
      "url": "http://localhost:5094/mcp",
      "type": "http"
    }
  }
}
```

**Stdio transport** — VS Code launches the server as a local process:

```json
{
  "mcpServers": {
    "scriptbee-docker": {
      "command": "docker",
      "args": [
        "run",
        "-i",
        "--rm",
        "-e",
        "GatewayApiUrl=http://host.docker.internal:5117",
        "scriptbee-mcp",
        "--stdio"
      ]
    }
  }
}
```

For the full configuration reference, see the [MCP Server README](Integrations/MCP/README.md).

---

## Contributing

For information on the repository structure, development setup, and how to contribute, please refer to our
*[Contributing Guide](.github/CONTRIBUTING.md)*.
