# Helper Functions Plugin

Helper Functions are a way to extend the functionality of ScriptBee. They can be used to add custom functions that can
be used in the scripts directly.

## Manifest

An example can be seen below:

```yaml title="manifest.yaml"
extensionPoints:
  - kind: HelperFunctions
    entryPoint: HelperFunction.dll
    version: 1.0.0
```

- `kind`: The type of plugin
- `entryPoint`: The relative path to the DLL containing the implemented interfaces for the respective plugins.
- `version`: The version of the plugin

## Helper Function Interface

```csharp title="IHelperFunction.cs"
public interface IHelperFunctions : IPlugin
{
    Task OnLoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    Task OnUnloadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
```

- `OnLoadAsync`: Called before the script is run
- `OnUnloadAsync`: Called after the script is run

### Example

ScriptBee has helper functions that deal with console output, csv files, json files and text files.

ScriptBee's default helper functions can be
found [here](https://github.com/dxworks/scriptbee/tree/master/Plugins/HelperFunctions/DxWorks.ScriptBee.Plugin.HelperFunctions.Default)
