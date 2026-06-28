# Change Log

All notable changes to the "scriptbee" extension will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.0]

### Added

- `scriptbee.enableLiveUpdates` to enable live updates for project script events of creation, update, and deletion.
- `scriptbee.enableAutoPush` to enable auto push to server when script is saved.

### Changes

- upgrade `axios` to 1.18.1
- upgrade `vscode-engine` to 1.125.0

## [1.0.0]

### Added

- Activity Bar Integration with ScriptBee explorer.
- Support for multiple connections to ScriptBee instances.
- Project Selection for active connections.
- Context menu for syncing, pulling, and pushing scripts between local machine and server.
- Support for opening project folder natively.
- Compare with Remote native diff viewer.
- Class generation command support for instances.
- Context menu item for pushing the currently active or selected script to the server.
- Context menu item for pulling the currently active or selected script from the server.
