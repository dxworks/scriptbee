# C# Analysis Service SDK

The `DxWorks.ScriptBee.Analysis.Sdk` NuGet package is the C# implementation of the
[Analysis Service SDK](../index). It lets you build a fully-featured Analysis Service in .NET
without writing any HTTP boilerplate.

## Installation

```bash
dotnet add package DxWorks.ScriptBee.Analysis.Sdk
```

Or directly in your `.csproj`:

```xml
<PackageReference Include="DxWorks.ScriptBee.Analysis.Sdk" Version="1.0.0" />
```

## What the SDK provides

| Layer            | What's included                                                            |
| :--------------- | :------------------------------------------------------------------------- |
| **Endpoints**    | All REST routes pre-wired with correct HTTP verbs, paths, and status codes |
| **Contracts**    | `Web*` request/response records matching the public REST API               |
| **Validators**   | FluentValidation rules for all request models                              |
| **Abstractions** | Use-case interfaces your service must implement                            |
| **Utilities**    | `FileBundler` for packaging file streams for loader plugins                |
| **Registration** | `AddAnalysisEndpoints()` extension to mount everything in one call         |

## Implementing the use cases

Implement each interface from the `DxWorks.ScriptBee.Analysis.Sdk` namespace. The SDK resolves them
from the DI container at runtime.

| Interface                     | Responsibility                              |
| :---------------------------- | :------------------------------------------ |
| `IRunAnalysisUseCase`         | Execute a script against the loaded context |
| `IGetContextUseCase`          | Return current context slices               |
| `IGetContextGraphUseCase`     | Return context as a graph (nodes + edges)   |
| `ILoadContextUseCase`         | Load files into context via a loader plugin |
| `ILinkContextUseCase`         | Link loaded context via a linker plugin     |
| `IClearContextUseCase`        | Clear all loaded context                    |
| `IGenerateClassesUseCase`     | Stream generated model class files          |
| `IGetInstalledPluginsUseCase` | List installed plugins                      |
| `IInstallPluginUseCase`       | Install a plugin by id                      |
| `IUninstallPluginUseCase`     | Uninstall a plugin by id                    |

## Supporting SDK interfaces

The SDK also exposes a small set of helper interfaces for context, scripts, model files, and results.
These are injected into the service layer and provide the low-level persistence and retrieval operations
that the higher-level use cases rely on.

### `IModelFileLoader`

```csharp
public interface IModelFileLoader
{
    Task<Stream> LoadModelFileStreamAsync(FileId fileId, CancellationToken cancellationToken);
}
```

Loads a model file stream identified by a `FileId` so the service can read or process the raw model file.

### `IScriptLoader`

```csharp
public interface IScriptLoader
{
    Task<OneOf<Script, ScriptDoesNotExistsError>> Get(
        ScriptId scriptId,
        CancellationToken cancellationToken
    );

    Task<OneOf<string, ScriptDoesNotExistsError>> GetScriptContent(
        ProjectId projectId,
        string path,
        CancellationToken cancellationToken
    );
}
```

Retrieves a script definition by `ScriptId`, and optionally resolves the script source text from a project
path.

### `IAnalysisState`

```csharp
public interface IAnalysisState
{
    Task<AnalysisInfo> CreateAsync(AnalysisInfo analysisInfo, CancellationToken cancellationToken);

    Task UpdateAsync(AnalysisInfo analysisInfo, CancellationToken cancellationToken);
}
```

Creates or updates persisted analysis metadata so the service can track the current analysis state.

### `IScriptResultsStore`

The current interface name is `IScriptResultsStore`. Older release notes and documentation may refer to
this as `IScriptResults`.

```csharp
public interface IScriptResultsStore
{
    Task UploadFileAsync<TMetadata>(
        FileId fileId,
        Stream fileStream,
        TMetadata? metadata = null,
        CancellationToken cancellationToken = default
    )
        where TMetadata : class;
}
```

Uploads a script result file and optional metadata for persistence under the provided `FileId`.
The SDK deliberately does not provide an in-memory or built-in results store; storage is something
that the concrete implementer offers by implementing `IScriptResultsStore` (backed by the host's file system,
blob storage, database, etc.).

### Example

```csharp
public sealed class Neo4jRunAnalysisService : IRunAnalysisUseCase
{
    private readonly INeo4jSession _session;

    public Neo4jRunAnalysisService(INeo4jSession session)
    {
        _session = session;
    }

    public async Task<WebRunAnalysisResponse> RunAsync(
        WebRunAnalysisCommand command,
        CancellationToken cancellationToken)
    {
        // store results in Neo4j, return response
        return new WebRunAnalysisResponse(...);
    }
}
```

## Registration

In your `Program.cs`, register each implementation and call the SDK extension method:

```csharp
builder.Services.AddScoped<IRunAnalysisUseCase, Neo4jRunAnalysisService>();
builder.Services.AddScoped<IGetContextUseCase, Neo4jGetContextService>();
// ... all other use cases

var app = builder.Build();
app.AddAnalysisEndpoints();
app.Run();
```

`AddAnalysisEndpoints()` registers all FluentValidation validators and maps every endpoint to
`IEndpointRouteBuilder` automatically.

### `IAnalysisResultService`

The high-level result service lets your analysis logic emit typed results (files, console output, errors,
or custom types) without dealing with `FileId`s, GUIDs, or stream management directly.

```csharp
public interface IAnalysisResultService
{
    Task<ResultId> AddFileAsync(
        string name, Stream content,
        string resultType = RunResultTypes.File,
        CancellationToken cancellationToken = default);

    Task<ResultId> AddFileAsync(
        string name, string content,
        string resultType = RunResultTypes.File,
        CancellationToken cancellationToken = default);

    Task<ResultId> AddConsoleAsync(
        string content, string name = "ConsoleOutput",
        CancellationToken cancellationToken = default);

    Task<ResultId> AddErrorAsync(
        string message, string name = "RunError",
        CancellationToken cancellationToken = default);

    Task<ResultId> AddResultAsync(
        string name, string resultType, Stream content,
        CancellationToken cancellationToken = default);
}
```

Inject `IAnalysisResultService` into your use-case implementations and use the ergonomic helpers:

```csharp
public sealed class MyRunAnalysisUseCase(IAnalysisResultService results) : IRunAnalysisUseCase
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await results.AddConsoleAsync("Analysis started", cancellationToken: cancellationToken);

        await results.AddFileAsync(
            "summary.json",
            """{"count": 42}""",
            cancellationToken: cancellationToken
        );

        await using var csvStream = BuildCsvReport();
        await results.AddFileAsync("report.csv", csvStream, cancellationToken: cancellationToken);
    }
}
```

The well-known type constants are available on `RunResultTypes`:

| Constant                  | Value        |
| :------------------------ | :----------- |
| `RunResultTypes.File`     | `"File"`     |
| `RunResultTypes.Console`  | `"Console"`  |
| `RunResultTypes.RunError` | `"RunError"` |

You can also pass any custom string as `resultType` when calling `AddFileAsync` or `AddResultAsync`
to define your own result categories.

## Versioning & Changelog

The SDK follows [Semantic Versioning](https://semver.org/). Releases are tagged `analysis-sdk@<version>`
in the monorepo and published automatically to NuGet.

See [CHANGELOG.md](https://github.com/dxworks/scriptbee/blob/main/integrations/sdks/csharp/DxWorks.ScriptBee.Analysis.Sdk/CHANGELOG.md)
for the full release history.
