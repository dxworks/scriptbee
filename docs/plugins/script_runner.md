# Script Runner Plugin

ScriptBee has a default Bundle that contains script runners for C#, Python and JavaScript.

## Manifest

An example can be seen below:

```yaml title="manifest.yaml"
extensionPoints:
  - kind: ScriptRunner
    entryPoint: Runner.dll
    version: 1.0.0
    language: csharp
```

- `kind`: The type of plugin
- `entryPoint`: The relative path to the DLL containing the implemented interfaces for the respective plugins.
- `version`: The version of the plugin
- `language`: The programming language of the script runner

## Script Runner Interface

```csharp title="IScriptRunner.cs"
public interface IScriptRunner : IPlugin
{
    public string Language { get; }

    public Task RunAsync(IProject project, IHelperFunctionsContainer helperFunctionsContainer, string scriptContent,
        CancellationToken cancellationToken = default);
}
```

### Example

ScriptBee's default C# script runner can be
found [here](https://github.com/dxworks/scriptbee/blob/master/Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp/CSharpScriptRunner.cs)

ScriptBee's default Python script runner can be
found [here](https://github.com/dxworks/scriptbee/blob/master/Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Python/PythonScriptRunner.cs)

ScriptBee's default JavaScript script runner can be
found [here](https://github.com/dxworks/scriptbee/blob/master/Plugins/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript/JavascriptScriptRunner.cs)
