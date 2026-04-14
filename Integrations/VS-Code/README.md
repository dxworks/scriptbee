# ScriptBee VS Code Extension

ScriptBee is a tool that helps the analysis of different models with scripts written in C#, Javascript or Python.

This is the Official VS Code extension for [ScriptBee](https://github.com/dxworks/scriptbee).

## Features

- **Activity Bar Integration**: Access all your saved ScriptBee connections right from the Activity Bar.
- **Multiple Connections**: Connect to local, staging, or production ScriptBee instances and switch seamlessly.
- **Project Selection**: Browse and select a project for the currently active connection.
- **Script Syncing**: Push and pull scripts between your local machine and the ScriptBee server.
- **Project Folder Management**: Easily open your project's script folder in VS Code.
- **Status Bar Indicator**: Quickly see which connection and project you are currently working on.

### Storage

Sync metadata (`.sb.meta` files) is stored directly alongside your scripts within the project folder. This ensures that:

- Metadata is natively tied to your files, making synchronization reliable even in complex multi-root workspaces.
- **Hidden from VS Code**: These files are automatically added to VS Code's `files.exclude` setting, keeping your sidebar clean.
- **Important**: You should add `**/*.sb.meta` to your `.gitignore` file. These files are meant for local synchronization only and should not be committed to version control.
- The metadata maps local files to their remote counterparts on the ScriptBee server, allowing for efficient syncing and change detection.

### Comparison and Diffs

- **Compare with Remote**: Right-click any script in the explorer or editor and select `ScriptBee: Compare with Remote`. This opens a native VS Code diff view, allowing you to see exactly what changed on the server before you decide to pull or push.

## Configuration

This extension can be configured via VS Code settings:

- `scriptbee.workspaceRoot`: The root folder where ScriptBee projects and generated code are stored. Default is `~/.scriptbee`.

## Getting Started

1. Open the **ScriptBee** view in the Activity Bar (look for the terminal/ScriptBee icon).
2. Click **Add Connection** (`+` icon) or run `ScriptBee: Add Connection` from the command palette.
3. Provide a name (e.g., `Local`) and the URL of your ScriptBee backend (e.g., `http://localhost:5000`).
4. Click **Select Project** inside your connection in the Activity Bar to choose an active project.
5. Once a project is selected, you can use the Sync, Pull, and Push icons to manage your scripts.

## Commands

- `ScriptBee: Add Connection`
- `ScriptBee: Edit Connection`
- `ScriptBee: Switch Connection`
- `ScriptBee: Delete Connection`
- `ScriptBee: Select Project`
- `ScriptBee: Refresh Tree View`
- `ScriptBee: Sync Scripts`
- `ScriptBee: Pull Scripts`
- `ScriptBee: Push Scripts`
- `ScriptBee: Open Project Folder`

## Requirements

You need a running instance of the ScriptBee Backend API.

## Known Issues

- Please report issues on the [GitHub Repository](https://github.com/dxworks/scriptbee).

> See [CHANGELOG.md](CHANGELOG.md) for more information.
