# Model Context Protocol (MCP) Integration

ScriptBee provides an official MCP Server that exposes its capabilities—like project management, data context loading, and script analysis—to AI clients (e.g. Claude Code, Cursor, and VS Code).

## Requirements

To use the MCP server, you must have the ScriptBee backend running.

## Connecting Clients

### Using HTTP Transport

When running the ScriptBee MCP server via `dotnet run`, it defaults to HTTP transport. Clients can connect to the `/mcp` endpoint.

For clients that support connecting to a remote MCP server natively via a URL (such as GitHub Copilot / VS Code via its MCP extension), you can configure the connection directly in your `mcp.json` like this:

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

_(Replace `5094` with the actual port the MCP server is running on)_

### Using Stdio Transport (For IDEs and Claude Code)

For local integrations where the client spawns the server directly, you can run the MCP server with the `--stdio` flag.

#### Configuration (`mcp.json`)

To configure a client like Claude Code or VS Code to use the ScriptBee MCP server, you should point directly to the compiled executable.

After building/publishing the MCP server (e.g., `dotnet publish -c Release`), add the following to your `mcp.json` or equivalent configuration file:

```json
{
  "mcpServers": {
    "scriptbee": {
      "command": "/absolute/path/to/ScriptBee.MCP.exe",
      "args": ["--stdio"]
    }
  }
}
```

_(On macOS/Linux, the command would be the path to the `ScriptBee.MCP` binary without the `.exe` extension, or `dotnet` with the path to `ScriptBee.MCP.dll` as the first argument)._

## Capabilities

The ScriptBee MCP server exposes the following primitives to the AI:

- **Tools**: Perform actions like creating projects, executing scripts, loading/linking data context, and triggering analyses.
- **Resources**: Browse script source code, view analysis console outputs, and inspect the loaded instance context graph.
- **Prompts**: Built-in workflows that guide the AI through tasks, such as exploring a project's state or executing a full analysis run.

## Configuration

The ScriptBee MCP server can be configured via command-line arguments or environment variables. Below are the available options:

| Option        | Description                           | Default                 |
| ------------- | ------------------------------------- | ----------------------- |
| GatewayApiUrl | The URL of the ScriptBee Gateway API. | `http://localhost:5117` |
