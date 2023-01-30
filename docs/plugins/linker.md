# Linker Plugin

A linker plugin is responsible for connecting the models loaded by the [loader plugins](loader.md) by adding links
in [ScriptBee's Context](../projects/context.md).

## Manifest

An example can be seen below:

```yaml title="manifest.yaml"
extensionPoints:
  - kind: Linker
    entryPoint: Linker.dll
    version: 1.0.0
```

- `kind`: The type of plugin
- `entryPoint`: The relative path to the DLL containing the implemented interfaces for the respective plugins.
- `version`: The version of the plugin

## Linker Interface

Each linker must implement the `IModelLinker` interface.

It receives the entire [context](../projects/context.md) to modify it and add links between the models.

```csharp
public interface IModelLinker : IPlugin
{
    public Task LinkModel(Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> context,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default);

    public string GetName();
}
```

- `LinkModel`: Receives the entire context to modify it and add links between the models.
- `GetName`: Returns the name of the plugin

### Example

An example of a linker plugin can be
found [here](https://github.com/dxworks/software-assessment-scriptbee-plugin/blob/master/DxWorks.ScriptBee.Plugins.SoftwareAssessment/SoftwareAssessmentLinker.cs)
