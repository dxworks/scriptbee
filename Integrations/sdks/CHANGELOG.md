# Changelog

All notable changes to ScriptBee Analysis Service SDKs (C#, Python, TypeScript) are documented here.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] — Initial Release

### Added

#### C# SDK

- **Results SDK**: `IAnalysisResultService`, `AnalysisResultService`, and `RunResultTypes` for emitting typed analysis
  results (`File`, `Console`, `RunError`, or custom types) to `IScriptResultsStore`.
- Extension method `services.AddAnalysisResultService()` for registering the results service in ASP.NET Core dependency
  injection.

#### Python SDK

- **Storage Decoupling**: Removed `InMemoryScriptResultsStore` from the public SDK API; storage is strictly provided by
  the concrete host via the `ScriptResultsStore` protocol.

#### TypeScript SDK

- **Initial release of `@dxworks/scriptbee-analysis-sdk`**
- **Use-case interfaces**: `RunAnalysisUseCase`, `GetContextUseCase`, `GetContextGraphUseCase`, `LoadContextUseCase`,
  `LinkContextUseCase`, `ClearContextUseCase`, `GenerateClassesUseCase`, `GetInstalledPluginsUseCase`,
  `InstallPluginUseCase`, `UninstallPluginUseCase`.
- **Domain models & validation**: Zod schemas and inferred types for `AnalysisInfo`, `ContextSlice`,
  `ContextGraphNode/Edge/Result`, `Plugin`, `PluginManifest`, `PluginExtensionPoint`, `SampleCodeFile`.
- **Web contracts & validation**: Web models matching the public REST API specification.
- **Hono routers**: `analyses_router`, `context_router`, `plugins_router`.
- **Results SDK**: `AnalysisResultService`, `DefaultAnalysisResultService`, and `ScriptResultsStore` interface for
  decoupled result storage (no in-memory store is bundled in the SDK; storage is provided by the concrete host).
- **FileBundler**: Binary wire format encoder for streaming generated classes (`Uint8Array` / `DataView`).
- **`createAnalysisSdkRouter()`**: Factory function assembling all use cases into a mountable Hono application.

---

### Added

- **Endpoint definitions** for all Analysis Service routes:
    - `GET /context` — retrieve current context slices
    - `GET /context/graph` — retrieve context as a graph
    - `POST /context/load` — load files into context
    - `POST /context/link` — link loaded context
    - `DELETE /context` — clear context
    - `POST /context/generate-classes` — stream generated model class files
    - `POST /analysis/run` — execute an analysis script
    - `GET /plugins` — list installed plugins
    - `POST /plugins/install` — install a plugin
    - `DELETE /plugins/{pluginId}` — uninstall a plugin
- **Web contracts** (`Web*` records) matching the public REST API surface defined in `analysis_swagger.json`.

#### C# SDK

- **Use-case interfaces** that decouple the HTTP layer from business logic:
  `IRunAnalysisUseCase`, `IGetContextUseCase`, `IGetContextGraphUseCase`, `ILoadContextUseCase`,
  `ILinkContextUseCase`, `IClearContextUseCase`, `IGenerateClassesUseCase`,
  `IGetInstalledPluginsUseCase`, `IInstallPluginUseCase`, `IUninstallPluginUseCase`.
- **`FileBundler`** utility for packaging files into a zip stream for loader plugins.
- **`AnalysisServiceCollectionExtensions.AddAnalysisEndpoints()`** — registers validators and maps all endpoints in a
  single call.
- **SDK interfaces**:
    - IModelFileLoader
    - IScriptResultsStore
    - IScriptLoader
    - IAnalysisState

#### Python SDK

- **Use-case ABCs** mirroring the C# interfaces: `IClearContextUseCase`, `IGenerateClassesUseCase`,
  `IGetContextGraphUseCase`, `IGetContextUseCase`, `IGetInstalledPluginsUseCase`, `IInstallPluginUseCase`,
  `ILinkContextUseCase`, `ILoadContextUseCase`, `IRunAnalysisUseCase`, `IUninstallPluginUseCase`.
- **Domain models** (Pydantic): `AnalysisInfo`, `ContextSlice`, `ContextGraphNode/Edge/Result`, `Plugin`,
  `PluginManifest`, `PluginExtensionPoint` discriminated unions, `SampleCodeFile`.
- **Web contracts** (Pydantic) matching the C# contracts with field validation.
- **FastAPI routers** for all 11 endpoints organized as `analyses_router`, `context_router`, `plugins_router`.
- **FileBundler** implementation byte-for-byte compatible with the C# version using Python's `struct` module.
- **`create_analysis_sdk_router()`** — assembles all routers into a single FastAPI `APIRouter`.
