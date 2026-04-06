# DxWorks.ScriptBee.Plugin.Api

This is the API for the ScriptBee plugin. It contains the interfaces and classes that are used by the plugin to
communicate with the ScriptBee service.

For entire documentation check [ScriptBee Plugin API](https://dxworks.org/scriptbee/plugins/plugin_api.html)

Currently, ScriptBee supports the following types of plugins:

- [Loader Plugins](https://dxworks.org/scriptbee/plugins/loader.html)
- [Linker Plugins](https://dxworks.org/scriptbee/plugins/linker.html)
- [Helper Functions Plugins](https://dxworks.org/scriptbee/plugins/helper_functions.html)
- [Script Generator Plugins](https://dxworks.org/scriptbee/plugins/script_generator.html)
- [Script Runner Plugins](https://dxworks.org/scriptbee/plugins/script_runner.html)
- [Bundle Plugins](https://dxworks.org/scriptbee/plugins/bundle.html)

Every plugin must have a `manifest.yaml` file in its root directory. More information about the manifest can be found in
the [manifest section](https://dxworks.org/scriptbee/plugins/manifest.html).

## Services

Besides the plugin interfaces, ScriptBee offers a set of services that can be used by plugins via dependency injection.

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

[ScriptRunner](https://dxworks.org/scriptbee/plugins/script_runner.html) uses the `IHelperFunctionsContainer` service to
get the helper functions and add them
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
