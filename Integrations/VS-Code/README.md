# ScriptBee VS Code Extension

ScriptBee is a tool that helps the analysis of different models with scripts written in C#, Javascript or Python.

This is the Official VS Code extension for [ScriptBee](https://github.com/dxworks/scriptbee).

For full documentation, visit [dxworks.org/scriptbee](https://dxworks.org/scriptbee).

## Features

- **Activity Bar Integration**: Access all your saved ScriptBee connections right from the Activity Bar.
- **Multiple Connections**: Connect to local, staging, or production ScriptBee instances and switch seamlessly.
- **Project Selection**: Browse and select a project for the currently active connection.
- **Instance Selection**: Choose an active analysis instance for instance-specific operations.
- **Script Syncing**: Push and pull scripts between your local machine and the ScriptBee server.
- **Single Script Push/Pull**: Right-click any script in the editor or explorer to push or pull just that file.
- **Compare with Remote**: Open a native VS Code diff view to see exactly what changed on the server.
- **Project Folder Management**: Easily open your project's script folder in VS Code.
- **Generate Classes**: Trigger model class generation from a running ScriptBee instance.

### Script Synchronization

| Command          | Description                                                       |
| ---------------- | ----------------------------------------------------------------- |
| **Sync Scripts** | Pulls all remote scripts, then pushes all local scripts.          |
| **Pull Scripts** | Downloads all scripts from the server, overwriting local changes. |
| **Push Scripts** | Uploads all local scripts to the server.                          |
| **Pull Script**  | Pulls the content of the currently active/selected script only.   |
| **Push Script**  | Pushes the content of the currently active/selected script only.  |

**Pull Script** and **Push Script** are available from both the **Editor** and **Explorer** right-click context menus.

### Sync Metadata

Sync metadata (`.sb.meta` files) is stored directly alongside your scripts within the project folder. This ensures that:

- Metadata is natively tied to your files, making synchronization reliable even in complex multi-root workspaces.
- **Hidden from VS Code**: These files are automatically added to VS Code's `files.exclude` setting, keeping your sidebar clean.
- **Important**: You should add `**/*.sb.meta` to your `.gitignore` file. These files are meant for local synchronization only and should not be committed to version control.
- The metadata maps local files to their remote counterparts on the ScriptBee server, allowing for efficient syncing and change detection.

### Comparison and Diffs

**Compare with Remote**: Right-click any script in the explorer or editor and select `ScriptBee: Compare with Remote`. This opens a native VS Code diff view, allowing you to see exactly what changed on the server before you decide to pull or push.

## Configuration

This extension can be configured via VS Code settings:

| Setting                   | Description                                                             | Default        |
| ------------------------- | ----------------------------------------------------------------------- | -------------- |
| `scriptbee.workspaceRoot` | The root folder where ScriptBee projects and generated code are stored. | `~/.scriptbee` |

## Getting Started

1. Open the **ScriptBee** view in the Activity Bar (look for the ScriptBee icon).
2. Click **Add Connection** (`+` icon) or run `ScriptBee: Add Connection` from the command palette.
3. Provide a name (e.g., `Local`) and the URL of your ScriptBee backend (e.g., `http://localhost:5000`).
4. Click **Select Project** inside your connection in the Activity Bar to choose an active project.
5. Once a project is selected, you can use the Sync, Pull, and Push icons to manage your scripts.

## Commands

| Command                          | Description                                          |
| -------------------------------- | ---------------------------------------------------- |
| `ScriptBee: Add Connection`      | Add a new ScriptBee backend connection.              |
| `ScriptBee: Edit Connection`     | Edit an existing connection's name or URL.           |
| `ScriptBee: Switch Connection`   | Make a connection the active one.                    |
| `ScriptBee: Delete Connection`   | Remove a connection.                                 |
| `ScriptBee: Select Project`      | Choose an active project for the current connection. |
| `ScriptBee: Select Instance`     | Choose an active analysis instance.                  |
| `ScriptBee: Refresh Tree View`   | Reload the connections panel.                        |
| `ScriptBee: Sync Scripts`        | Pull then push all scripts.                          |
| `ScriptBee: Pull Scripts`        | Pull all scripts from the server.                    |
| `ScriptBee: Pull Script`         | Pull the current script from the server.             |
| `ScriptBee: Push Scripts`        | Push all scripts to the server.                      |
| `ScriptBee: Push Script`         | Push the current script to the server.               |
| `ScriptBee: Compare with Remote` | Open a diff view between local and remote script.    |
| `ScriptBee: Open Project Folder` | Open the project's local script folder.              |
| `ScriptBee: Generate Classes`    | Generate model classes from the active instance.     |

## Requirements

You need a running instance of the ScriptBee Backend API. See the [installation guide](https://dxworks.org/scriptbee/installation/installation) for setup instructions.

## Known Issues

Please report issues on the [GitHub Repository](https://github.com/dxworks/scriptbee/issues).

> See [CHANGELOG.md](CHANGELOG.md) for version history.
