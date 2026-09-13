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

## Versioning & Changelog

The SDK follows [Semantic Versioning](https://semver.org/). Releases are tagged `analysis-sdk@<version>`
in the monorepo and published automatically to NuGet.

See [CHANGELOG.md](https://github.com/dxworks/scriptbee/blob/main/integrations/sdks/csharp/DxWorks.ScriptBee.Analysis.Sdk/CHANGELOG.md)
for the full release history.
