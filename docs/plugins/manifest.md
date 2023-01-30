# Plugin Manifest

Each plugin folder must have a `manifest.yaml` file that describes the plugin. If the file is missing, the plugin will
not be loaded.

ScriptBee supports the following plugin types:

- [Loader](loader.md)
- [Linker](linker.md)
- [Script Generator](script_generator.md)
- [Script Runner](script_runner.md)
- [Helper Functions](helper_functions.md)
- [Bundle Plugins](bundle.md)

Some examples of a plugin manifests can be
found [here](https://github.com/dxworks/scriptbee/tree/master/ScriptBee.Tests/Plugin/Manifest/TestData).

## Anatomy of a Manifest

The manifest file describes how the plugin is structured and what it does. It can contain multiple extension points

```yaml title="manifest.yaml"
apiVersion: 1.0.0
author: ScriptBee
description: "Description"
name: "HelperFunctions example"
extensionPoints:
  - kind: HelperFunctions
    entryPoint: Plugin.dll
    version: "0.0.1"
```

- `apiVersion`: The version of the plugin manifest api
- `author`: The author of the plugin
- `description`: A description of the plugin
- `name`: The name of the plugin
- `extensionPoints`: A list of extension points that the plugin provides
- `kind`: The type of plugin
- `entryPoint`: If the extension point is a plugin, the entry point is the relative path to the DLL containing the
  implemented interfaces for the respective plugins. If the extension point is a bundle, the entryPoint is the id of
  the plugin
- `version`: The version of the plugin

> Note: depending on the plugin type, each extension point can have different properties
