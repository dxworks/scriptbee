# Plugin Installation

ScriptBee supports different types of plugins, as well as plugin bundles which are a collection of plugins.
They are installed in the plugins folder of the ScriptBee data folder. For more information about the ScriptBee data
folder, see [Installation section](../home/installation.md).

In order for a plugin to be considered valid, the folder must contain a `manifest.yaml` file.

> Note: when a new version of a plugin is installed, the old version will be removed.

## Automatic Installation

Plugins and Bundles can be installed automatically using the UI from the `plugins` section.

The plugin information are fetched from the [DxWorks Hub](https://github.com/dxworks/dxworks-hub).
ScriptBee uses the [DxWorks Hub SDK](https://github.com/dxworks/dxworks-hub-sdk) to communicate with the Hub.
It will create a folder called `.dxw` in the users folder were the hub will be downloaded and used. (
e.g. `C:\Users\{user}\.dxw\hub`)

The Sdk will clone the DxWorks Hub repository and will use the `hub` folder to get the information about the plugins
like the name, version and others.

## Manual Installation

Plugins and Bundles can be installed manually by copying the plugin folders into the `plugins` folder of the ScriptBee
data folder.

The folder of the plugin must be named with the following format: `{PluginName}@{PluginVersion}` and must contain
a `manifest.yaml` file.

## Uninstalling Plugins

Plugins can be uninstalled from the `plugins` section of the UI.
When a plugin is uninstalled, the web server will needs to be restarted in order to take effect.
