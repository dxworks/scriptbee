# Loader Plugin

A loader plugin is responsible for loading raw, serialized data files into a model that will be added
in [ScriptBee's Context](../projects/context.md).

## Manifest

An example can be seen below:

```yaml title="manifest.yaml"
extensionPoints:
  - kind: Loader
    entryPoint: Loader.dll
    version: 1.0.1
```

- `kind`: The type of plugin
- `entryPoint`: The relative path to the DLL containing the implemented interfaces for the respective plugins.
- `version`: The version of the plugin

## Loader Interface

Each loader plugin must implement the `IModelLoader` interface.

It receives a list of streams containing the raw data files and returns a dictionary of models.

```csharp
public interface IModelLoader : IPlugin
{
    public Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(List<Stream> fileStreams,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default);

    public string GetName();
}
```

- `LoadModel`: Receives a list of streams containing the raw data files and returns a dictionary of models.
- `GetName`: Returns the name of the plugin

The `LoadModel` method returns a dictionary of models. The key of the dictionary is the name of the exported entity and
the value is a dictionary that contains the models. The key of the inner dictionary (entities dictionary) is a unique
value specific to the loader plugin and the value is the model.

### Example

An example of a loader plugin can be
found
at [Jira Miner ScriptBee Plugin](https://github.com/dxworks/jira-scriptbee-plugin/blob/master/Dxworks.ScriptBee.Plugins.JiraMiner/Loaders/ModelLoader.cs)
or
at [Honeydew ScriptBee Plugin](https://github.com/dxworks/honeydew/blob/master/DxWorks.ScriptBee.Plugins.Honeydew/Loaders/ModelLoader.cs)
