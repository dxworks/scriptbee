# VS Code Extension

The **ScriptBee VS Code Extension** brings the full power of ScriptBee directly into your editor.
It allows you to manage connections, select projects, and synchronize scripts between your local
machine and a running ScriptBee backend — all without leaving VS Code.

## Installation

### Via the VS Code Marketplace (Recommended)

1. Open VS Code.
2. Go to the **Extensions** view (`Ctrl+Shift+X` / `Cmd+Shift+X`).
3. Search for **ScriptBee**.
4. Click **Install** on the extension published by **dxworks**.

Alternatively, you can install it directly from the command palette:

```
ext install dxworks.scriptbee
```

### Via VSIX (Direct Download)

If you prefer to install a specific version or a pre-release build, you can download the `.vsix` file
directly from the [GitHub Releases page](https://github.com/dxworks/scriptbee/releases).

Once downloaded:

1. Open VS Code.
2. Go to the **Extensions** view (`Ctrl+Shift+X`).
3. Click the `...` menu (top-right of the panel) and select **Install from VSIX...**.
4. Select the downloaded `.vsix` file.

## Features

### Activity Bar Integration

A dedicated **ScriptBee panel** is added to the VS Code Activity Bar. From here you can:

- View all configured connections at a glance.
- See which connection is currently active (marked with a checkmark icon).
- Navigate to the selected project and running instance.

### Connection Management

You can define multiple connections to different ScriptBee backends (e.g., local development, staging, production).

| Action                   | How                                                 |
| ------------------------ | --------------------------------------------------- |
| Add a connection         | Click the `+` icon in the ScriptBee panel title bar |
| Edit a connection        | Click the pencil icon next to a connection          |
| Switch active connection | Click the checkmark icon next to a connection       |
| Delete a connection      | Click the trash icon next to a connection           |

### Project Selection

Each connection can have an active project. Click the **Select Project** gear icon (`⚙`) on a connection
to browse and select from the projects available on that ScriptBee backend.

### Instance Selection

Once a project is selected, you can select an active **analysis instance** from the instances running
on the server. This is used for instance-specific operations like class generation.

### Script Synchronization

The extension provides full two-way synchronization of scripts between your local machine and the
ScriptBee server. Scripts are stored locally under `~/.scriptbee/projects/<projectId>/src/` by default.

| Command          | Description                                                                |
| ---------------- | -------------------------------------------------------------------------- |
| **Sync Scripts** | Pulls all remote scripts, then pushes all local scripts.                   |
| **Pull Scripts** | Downloads all scripts from the server, overwriting local changes.          |
| **Push Scripts** | Uploads all local scripts to the server.                                   |
| **Pull Script**  | Pulls the content of the currently selected/active script from the server. |
| **Push Script**  | Pushes the content of the currently selected/active script to the server.  |

**Pull Script** and **Push Script** are available via right-click in both the **Explorer sidebar**
and the **Editor** context menus.

### Live Updates

When enabled, the extension uses SignalR to listen for real-time script lifecycle events from the server.

- **Script Created**: Refreshes the connection tree view automatically.
- **Script Updated**: Automatically pulls the updated content for scripts already synced locally.
- **Script Deleted**: Removes the script's local metadata and refreshes the tree view.

### Compare with Remote

Right-click any script in the Explorer or Editor and select **ScriptBee: Compare with Remote** to
open a native VS Code diff view. This lets you review exactly what changed on the server before
deciding to pull or push.

### Open Project Folder

Click the **Open Project Folder** icon on a project item to open the local script folder directly
in VS Code's file explorer.

### Generate Classes

For an active instance, you can trigger **ScriptBee: Generate Classes** to generate C# model
class files from the loaded project context. Generated files are stored in the `.generated` folder.

## Configuration

The extension exposes the following settings via **VS Code Settings** (`Ctrl+,` / `Cmd+,`):

| Setting                       | Description                                                                                    | Default        |
| ----------------------------- | ---------------------------------------------------------------------------------------------- | -------------- |
| `scriptbee.workspaceRoot`     | The root folder where ScriptBee projects and generated code are stored.                        | `~/.scriptbee` |
| `scriptbee.enableLiveUpdates` | Enable real-time updates for script lifecycle events (creation, update, deletion) via SignalR. | `true`         |
| `scriptbee.enableAutoPush`    | Automatically push script changes to the server upon saving.                                   | `true`         |

To change these settings, open VS Code settings, search for **ScriptBee**, and update the desired fields.
You can set project-specific paths by saving the setting at the **Workspace** or **Folder** level.

## Sync Metadata

The extension creates `.sb.meta` files alongside your scripts to track the mapping between local
files and their remote counterparts on the server.

> [!IMPORTANT]
> Add `**/*.sb.meta` to your `.gitignore` file. These files are for local use only and should
> not be committed to version control.

These files are automatically hidden from the VS Code Explorer via the `files.exclude` setting.

## Getting Started

1. Open the **ScriptBee** panel in the Activity Bar.
2. Click **Add Connection** (`+`) and enter:
   - A **name** (e.g., `Local`)
   - The **URL** of your ScriptBee backend (e.g., `http://localhost:5000`)
3. The new connection will become active automatically.
4. Click **Select Project** to choose a project.
5. Use the **Sync**, **Pull**, or **Push** icons on the project item to manage your scripts.

## Requirements

A running instance of the ScriptBee Backend API is required. See the
[installation guide](./installation.md) for details on how to deploy ScriptBee.
