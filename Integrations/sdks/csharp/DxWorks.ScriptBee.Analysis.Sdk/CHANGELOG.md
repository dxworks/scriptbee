# Changelog

All notable changes to `DxWorks.ScriptBee.Analysis.Sdk` are documented here.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

## [1.0.0] — Initial Release

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
- **FluentValidation validators** for all request models.
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
