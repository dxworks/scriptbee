# Plugin Bundle

A bundle is a collection of plugins that are used together. It can contain also standalone extension points.

When a bundle is installed, the contained plugins are installed as well.

## Manifest

An example can be seen below:

```yaml title="manifest.yaml"
apiVersion: 1.0.0
author: ScriptBee
description: "Bundle Plugin"
name: Bundle Plugin
extensionPoints:
  - kind: HelperFunctions
    entryPoint: HelperFunctions.dll
    version: 1.0.0
  - kind: Plugin
    entryPoint: plugin-id-1
    version: 1.0.1
  - kind: Plugin
    entryPoint: plugin-id-2
    version: 5.0.1  
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

### Example

An example of a bundle can be
found
at [Software Assesment ScriptBee Plugin](https://github.com/dxworks/software-assessment-scriptbee-plugin/blob/master/manifest.yaml)
