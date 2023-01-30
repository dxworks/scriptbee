# ScriptBee's Plugin API

## DxWorks.ScriptBee.Plugin.Api

ScriptBee's plugin API is a way to extend the functionality of ScriptBee. Plugins can be used to add custom
functionality to ScriptBee.

Currently, ScriptBee supports the following plugins:

- [Loader Plugins](loader.md)
- [Linker Plugins](linker.md)
- [Helper Functions Plugins](helper_functions.md)
- [Script Generator Plugins](script_generator.md)
- [Script Runner Plugins](script_runner.md)
- [Bundle Plugins](bundle.md)

Every plugin must have a `manifest.yaml` file in its root directory. More information about the manifest can be found in
the [manifest section](manifest.md).

## Services

Besides the plugin interfaces, ScriptBee also offers a set of services that can be used by plugins via dependency
injection.

### IHelperFunctionsContainer

The `IHelperFunctionsContainer` service is used to register helper functions. Helper functions are a way to extend the
functionality of ScriptBee. They can be used to add custom functions that can be used in the scripts directly.

`IHelperFunctionsContainer` wraps the helper functions and provides a way to access them.

```csharp title="IHelperFunctionsContainer.cs"
public interface IHelperFunctionsContainer
{
    public Dictionary<string, Delegate> GetFunctionsDictionary();
    
    public IEnumerable<IHelperFunctions> GetFunctions();
}
```

[ScriptRunner](script_runner.md) uses the `IHelperFunctionsContainer` service to get the helper functions and add them
to the script engine.

### IHelperFunctionsResultService

The `IHelperFunctionsResultService` service is used to store the results of the helper functions and have a uniform way
to deal with script outputs.

```csharp title="IHelperFunctionsResultService.cs"
public interface IHelperFunctionsResultService
{
    Task UploadResultAsync(string fileName, string type, string content, CancellationToken cancellationToken = default);

    Task UploadResultAsync(string fileName, string type, Stream content, CancellationToken cancellationToken = default);

    void UploadResult(string fileName, string type, string content);

    void UploadResult(string fileName, string type, Stream content);
}

```

[ScriptBee's default helper functions](https://github.com/dxworks/scriptbee/tree/master/Plugins/HelperFunctions/DxWorks.ScriptBee.Plugin.HelperFunctions.Default)
use the `IHelperFunctionsResultService` service to upload the results of the
helper functions.

### Run Result Types

ScriptBee has a set of predefined result types that can be used by the helper functions to upload the results.

```csharp title="RunResultDefaultTypes.cs"
public static class RunResultDefaultTypes
{
    public const string ConsoleType = "Console";
    public const string FileType = "File";
    public const string RunError = "RunError";
}
```
