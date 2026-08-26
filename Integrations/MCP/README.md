# ScriptBee MCP Server

This is the official Model Context Protocol (MCP) server for ScriptBee, built using the standard `.NET` MCP SDK.
It exposes ScriptBee capabilities (project management, context loading, script execution, and analysis) to AI clients
like Claude Desktop, Claude Code, GitHub Copilot, or VS Code.

## Running the Server

You can run the server in two modes:

### HTTP Transport

```bash
cd src/ScriptBee.MCP
dotnet run
```

The server will start on `http://localhost:5094` (or the configured port) and expose the `/mcp` endpoint for SSE
connections.

For clients that support connecting to a remote MCP server natively via a URL (such as GitHub Copilot / VS Code via its
MCP extension), you can configure the connection directly in your `mcp.json` like this:

```json
{
  "mcpServers": {
    "scriptbee-mcp-server": {
      "url": "http://localhost:5094/mcp",
      "type": "http"
    }
  }
}
```

*(Replace `5094` with the actual port the MCP server is running on)*

### Stdio Transport (for IDEs and Claude Desktop)

```bash
cd src/ScriptBee.MCP
dotnet run -- --stdio
```

## Configuring Client Applications

### VS Code, Claude Code, or GitHub Copilot

You can add this server to your local tools using an `mcp.json` configuration file (or the IDE's corresponding settings
file).

Create or update your `mcp.json` with the following.

**Option 1: Using the Dockerfile (Recommended)**
After building or publishing the project, point directly to the output executable.

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

**Option 2: Running from source (Development)**

```json
{
  "mcpServers": {
    "scriptbee": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/Absolute/Path/To/ScriptBee.MCP.csproj",
        "--",
        "--stdio"
      ]
    }
  }
}
```

## Exposed Capabilities

### Tools

- **ProjectTools**: Manage projects (`GetProjects`, `CreateProject`, etc.)
- **ScriptTools**: Manage scripts and project files
- **InstanceTools**: Manage execution instances
- **ContextTools**: Load and link data contexts, search the context graph
- **AnalysisTools**: Trigger script analyses and fetch results/logs

### Resources

- **Script Source Code**: Read script contents (`scriptbee://projects/{projectId}/scripts/{scriptId}/content`)
- **Analysis Console Output**: Read analysis logs (`scriptbee://projects/{projectId}/analyses/{analysisId}/console`)
- **Instance Context**: View the loaded context graph summary (
  `scriptbee://projects/{projectId}/instances/{instanceId}/context`)

### Prompts

- **explore-project**: Guides the AI to give an overview of a project.
- **load-and-link-context**: Guides the AI through the context ingestion workflow.
- **run-analysis**: A workflow for the AI to run an analysis and monitor it until completion.

## Authentication

When the ScriptBee Gateway requires authentication, the MCP server can authenticate in several ways:

### 1. Direct Bearer Token (VS Code / Copilot / Standalone)

Provide a pre-existing JWT access token using the `--token` CLI argument or the `Authentication__AccessToken` / `ScriptBee__Authentication__AccessToken` environment variable.

```json
{
  "mcpServers": {
    "scriptbee": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/Absolute/Path/To/ScriptBee.MCP.csproj",
        "--",
        "--stdio",
        "--token",
        "ey...",
        "--gateway-url",
        "http://localhost:5117"
      ]
    }
  }
}
```

### 2. OIDC Client Credentials (Machine-to-Machine / CI)

Configure the MCP server to automatically acquire and refresh access tokens from an OpenID Connect (OIDC) identity provider (e.g. Keycloak):

```json
{
  "mcpServers": {
    "scriptbee": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/Absolute/Path/To/ScriptBee.MCP.csproj",
        "--",
        "--stdio",
        "--authority",
        "http://localhost:8080/realms/scriptbee",
        "--client-id",
        "scriptbee-mcp",
        "--client-secret",
        "secret-key",
        "--gateway-url",
        "http://localhost:5117"
      ]
    }
  }
}
```

## Configuration

The ScriptBee MCP server can be configured via command-line arguments or environment variables. Below are the available options:

| Option                          | CLI Argument      | Description                                                | Default                 |
|---------------------------------|-------------------|------------------------------------------------------------|-------------------------|
| `GatewayApiUrl`                 | `--gateway-url`   | The URL of the ScriptBee Gateway API.                      | `http://localhost:5117` |
| `Authentication:AccessToken`    | `--token`         | Static JWT access token for authenticating with Gateway.   | `null`                  |
| `Authentication:Authority`      | `--authority`     | OIDC Identity Provider authority URL.                      | `null`                  |
| `Authentication:ClientId`       | `--client-id`     | OIDC client ID for client credentials flow.                | `null`                  |
| `Authentication:ClientSecret`   | `--client-secret` | OIDC client secret for client credentials flow.            | `null`                  |
| `Authentication:Scope`          | `--scope`         | OIDC scope requested during token acquisition.             | `null`                  |
| `Authentication:TokenEndpoint`  | `--token-endpoint`| Explicit token endpoint URL (overrides OIDC discovery).    | `null`                  |
