# ScriptBee VS Code Extension

ScriptBee is a tool that helps the analysis of different models with scripts written in C#, Javascript or Python.

This is the Official VS Code extension for [ScriptBee](https://github.com/dxworks/scriptbee).

## Features

- **Activity Bar Integration**: Access all your saved ScriptBee connections right from the Activity Bar.
- **Multiple Connections**: Connect to local, staging, or production ScriptBee instances and switch seamlessly.
- **Project Selection**: Browse and select a project for the currently active connection.
- **Status Bar Indicator**: Quickly see which connection and project you are currently working on.

## Getting Started

1. Open the **ScriptBee** view in the Activity Bar (look for the terminal/ScriptBee icon).
2. Click **Add Connection** (`+` icon) or run `ScriptBee: Add Connection` from the command palette.
3. Provide a name (e.g., `Local`) and the URL of your ScriptBee backend (e.g., `http://localhost:5000`).
4. Click **Select Project** inside your connection in the Activity Bar to choose an active project.

## Commands

- `ScriptBee: Add Connection`
- `ScriptBee: Edit Connection`
- `ScriptBee: Switch Connection`
- `ScriptBee: Delete Connection`
- `ScriptBee: Select Project`
- `ScriptBee: Refresh Tree View`

## Requirements

You need a running instance of the ScriptBee Backend API.

## Known Issues

- Please report issues on the [GitHub Repository](https://github.com/dxworks/scriptbee).

> See [CHANGELOG.md](CHANGELOG.md) for more information.
