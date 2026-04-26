# Plugin Installation

ScriptBee supports different types of plugins, as well as plugin bundles which are a collection of plugins.
They are primarily stored in the **Plugin Cache**, which is part of the ScriptBee data folder. From there, they can be enabled for the Gateway Service to use. For more information about the ScriptBee data
folder, see [Installation section](../installation/installation.md).

In order for a plugin to be considered valid, the folder must contain a `manifest.yaml` file.

> Note: when a new version of a plugin is installed, the old version will be removed.

## Automatic Installation

Plugins and Bundles can be installed automatically using the dedicated **Marketplace** section in the UI.
Downloading a plugin from the Marketplace places it in the **Plugin Cache**. To make a plugin available for use in the Gateway (e.g., to use it as a Linker or Script Runner).

The plugin information are fetched from the [DxWorks Hub](https://github.com/dxworks/dxworks-hub).
ScriptBee uses the [DxWorks Hub SDK](https://github.com/dxworks/dxworks-hub-sdk) to communicate with the Hub.
It will create a folder called `.dxw` in the users folder were the hub will be downloaded and used. (
e.g. `C:\Users\{user}\.dxw\hub`)

The Sdk will clone the DxWorks Hub repository and will use the `hub` folder to get the information about the plugins
like the name, version and others.

## Manual Installation

From the project's **Marketplace Dashboard**, you can use the **Upload Plugin** button to upload a `.zip` file of your
plugin.

- The zip file must contain a `manifest.yaml` at its root.
- Plugins uploaded this way are **project-specific** and will only be available to the project you uploaded them to.
- They are stored in the `projects/{projectId}/plugins` folder.
- Conflicts are detected if the same version already exists globally or in the project.

## Uninstalling Plugins

Plugins can be uninstalled from the `plugins` section of the UI.
When a plugin is uninstalled, it is removed from the project and, if no other project is using it, its files are deleted from the disk and it is unloaded from memory.

The uninstallation takes effect **immediately** without requiring a server restart.
