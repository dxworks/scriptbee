# Analysis Service SDK

ScriptBee is designed so that the **Analysis Service** is a replaceable component. Instead of being locked into a single
in-memory implementation, any team can build and deploy their own Analysis Service — backed by a database, a graph
engine like Neo4j, a remote computation cluster, or anything else — as long as it exposes the expected REST API.

The **Analysis Service SDK** packages everything needed to build such a service without writing HTTP boilerplate. You
implement the business logic; the SDK handles routing, serialization, validation, and wiring.

## Concept

```
┌────────────────┐   REST API    ┌──────────────────────────────┐
│   Gateway      │ ─────────────▶│   Your Analysis Service      │
│ (ScriptBee)    │               │                              │
└────────────────┘               │  SDK endpoints + validators  │
                                 │  ┌──────────────────────┐    │
                                 │  │  Your implementation  │    │
                                 │  │  of the use cases     │    │
                                 │  └──────────────────────┘    │
                                 └──────────────────────────────┘
```

The Gateway is the only client of the Analysis Service. It communicates exclusively through the
[public REST API](../architecture/analysis_rest_api). The SDK fully implements that API surface.

## Use Cases

Every Analysis Service must support the following capabilities:

| Capability                | Description                                           |
| :------------------------ | :---------------------------------------------------- |
| **Run Analysis**          | Execute a script against the currently loaded context |
| **Get Context**           | Return all currently loaded context slices            |
| **Get Context Graph**     | Return the context as a graph (nodes + edges)         |
| **Load Context**          | Load files into the context using a loader plugin     |
| **Link Context**          | Link the loaded context using a linker plugin         |
| **Clear Context**         | Dispose of all loaded context                         |
| **Generate Classes**      | Stream generated model class files for IDE support    |
| **Get Installed Plugins** | List all plugins available in this service instance   |
| **Install Plugin**        | Install a plugin by identifier                        |
| **Uninstall Plugin**      | Remove an installed plugin                            |

## Available SDK Implementations

| Language                                | Package                                   | Status       |
| :-------------------------------------- | :---------------------------------------- | :----------- |
| [C#](./csharp_analysis_service_sdk)         | `DxWorks.ScriptBee.Analysis.Sdk` on NuGet | ✅ Available |
| [Python](./python_analysis_service_sdk)     | `scriptbee-analysis-sdk` on PyPI          | ✅ Available |
| [TypeScript](./typescript_analysis_service_sdk) | `@dxworks/scriptbee-analysis-sdk` on npm  | ✅ Available |

---

## Results SDK

In addition to the request/response endpoints, all SDK implementations provide a **Results SDK** —
a high-level, write-only sink that lets analysis logic emit typed results without managing low-level
concerns such as file IDs, GUIDs, byte encoding, or store calls.

> **Storage Decoupling**: The SDKs deliberately do not include any in-memory or built-in storage implementations.
> The SDKs only offer the basic contracts and abstractions; storage is something that the concrete implementer
> offers (by implementing `IScriptResultsStore` in C# or `ScriptResultsStore` in Python and TypeScript, backed by
> e.g. S3, disk, blob storage, or a database).


| Result type   | Produced by                                    | Consumed by       |
| :------------ | :--------------------------------------------- | :---------------- |
| `File`        | `AddFileAsync` / `add_file` / `addFile`        | UI file downloads |
| `Console`     | `AddConsoleAsync` / `add_console` / `addConsole`| Console log view  |
| `RunError`    | `AddErrorAsync` / `add_error` / `addError`     | Error display     |
| Custom string | `AddResultAsync` / `add_result` / `addResult`  | Plugin-defined    |

The interface is the same conceptually across all languages:

- **C#**: `IAnalysisResultService` with `AddFileAsync`, `AddConsoleAsync`, `AddErrorAsync`, `AddResultAsync`
- **Python**: `AnalysisResultService` protocol with `add_file`, `add_console`, `add_error`, `add_result`
- **TypeScript**: `AnalysisResultService` interface with `addFile`, `addConsole`, `addError`, `addResult`

See the language-specific SDK page for usage examples and DI registration details.

---

> **Note**: All SDK implementations target the same REST API contract defined in
> [`docs/public/analysis_swagger.json`](../public/analysis_swagger.json).
> The Gateway does not know or care which language or backing store your service uses.
