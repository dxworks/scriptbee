# TypeScript Analysis Service SDK

The `@dxworks/scriptbee-analysis-sdk` package is the TypeScript implementation of the
[Analysis Service SDK](./index). It lets you build a fully-featured Analysis Service in TypeScript
using [Hono](https://hono.dev/) without writing HTTP boilerplate.

## Installation

```bash
npm install @dxworks/scriptbee-analysis-sdk hono zod
```

## What the SDK provides

| Layer            | What's included                                                            |
| :--------------- | :------------------------------------------------------------------------- |
| **Endpoints**    | All REST routes pre-wired with correct HTTP verbs, paths, and status codes |
| **Contracts**    | `Web*` Zod schemas and request/response models matching the REST API       |
| **Validators**   | Zod validation for all request payloads                                    |
| **Abstractions** | Type-safe interfaces your service implements                               |
| **Utilities**    | `FileBundler` for packaging binary file streams for loader plugins         |
| **Registration** | `createAnalysisSdkRouter()` to mount everything into a Hono application    |

## Architecture

```
┌────────────────┐   REST API    ┌────────────────────────────────────┐
│   Gateway      │ ─────────────▶│    Your Hono Analysis Service      │
│ (ScriptBee)    │               │                                    │
└────────────────┘               │  createAnalysisSdkRouter()         │
                                 │  ┌────────────────────────────┐    │
                                 │  │  Your AnalysisSdkContainer │    │
                                 │  │  (10 use-case properties)  │    │
                                 │  └────────────────────────────┘    │
                                 └────────────────────────────────────┘
```

## Implementing the use cases

Implement each use case interface from `@dxworks/scriptbee-analysis-sdk` and expose them through
an object conforming to `AnalysisSdkContainer`.

| Interface                    | Responsibility                              |
| :--------------------------- | :------------------------------------------ |
| `RunAnalysisUseCase`         | Execute a script against the loaded context |
| `GetContextUseCase`          | Return current context slices               |
| `GetContextGraphUseCase`     | Return context as a graph (nodes + edges)   |
| `LoadContextUseCase`         | Load files into context via a loader plugin |
| `LinkContextUseCase`         | Link loaded context via a linker plugin     |
| `ClearContextUseCase`        | Clear all loaded context                    |
| `GenerateClassesUseCase`     | Stream generated model class files          |
| `GetInstalledPluginsUseCase` | List installed plugins                      |
| `InstallPluginUseCase`       | Install a plugin by id                      |
| `UninstallPluginUseCase`     | Uninstall a plugin by id                    |

### Example implementation

```typescript
import type {
  AnalysisInfo,
  RunAnalysisCommand,
  RunAnalysisUseCase,
} from "@dxworks/scriptbee-analysis-sdk";

export class Neo4jRunAnalysisService implements RunAnalysisUseCase {
  constructor(private readonly session: unknown) {}

  async run(command: RunAnalysisCommand): Promise<AnalysisInfo> {
    return {
      id: { value: "analysis-1" },
      projectId: command.projectId,
      scriptId: command.scriptId,
      status: { value: "Running" },
      creationDate: new Date(),
    };
  }
}
```

## Registration

Create a container implementing `AnalysisSdkContainer`, then call `createAnalysisSdkRouter()`
to get a pre-configured Hono app:

```typescript
import { Hono } from "hono";
import { serve } from "@hono/node-server";
import {
  createAnalysisSdkRouter,
  type AnalysisSdkContainer,
} from "@dxworks/scriptbee-analysis-sdk";

class AppContainer implements AnalysisSdkContainer {
  readonly runAnalysis = new Neo4jRunAnalysisService(null);
  readonly getContext = new Neo4jGetContextService();
  readonly loadContext = new Neo4jLoadContextService();
  readonly linkContext = new Neo4jLinkContextService();
  readonly clearContext = new Neo4jClearContextService();
  readonly getContextGraph = new Neo4jContextGraphService();
  readonly generateClasses = new Neo4jGenerateClassesService();
  readonly getInstalledPlugins = new Neo4jInstalledPluginsService();
  readonly installPlugin = new Neo4jInstallPluginService();
  readonly uninstallPlugin = new Neo4jUninstallPluginService();
}

const app = new Hono();
app.route("/", createAnalysisSdkRouter(new AppContainer()));

serve({
  fetch: app.fetch,
  port: 5000,
});
```

`createAnalysisSdkRouter()` registers all endpoints under `/api/` and routes each request to the
corresponding use case in the container.

## Container interface

The `AnalysisSdkContainer` interface defines all 10 use-case properties:

```typescript
export interface AnalysisSdkContainer {
  readonly runAnalysis: RunAnalysisUseCase;
  readonly getContext: GetContextUseCase;
  readonly loadContext: LoadContextUseCase;
  readonly linkContext: LinkContextUseCase;
  readonly clearContext: ClearContextUseCase;
  readonly getContextGraph: GetContextGraphUseCase;
  readonly generateClasses: GenerateClassesUseCase;
  readonly getInstalledPlugins: GetInstalledPluginsUseCase;
  readonly installPlugin: InstallPluginUseCase;
  readonly uninstallPlugin: UninstallPluginUseCase;
}
```

## FileBundler

The `FileBundler` utility serialises a list of `SampleCodeFile` objects into the binary format
consumed by the Gateway for the generate-classes endpoint.

```typescript
import { FileBundler } from "@dxworks/scriptbee-analysis-sdk";

const bundler = new FileBundler();
const buffer = bundler.writeToBuffer([
  { name: "Model.ts", content: "export interface Model {}" },
]);
```

The wire format matches the Gateway expectation: for each file, a big-endian `uint32` path length,
the UTF-8 encoded path, a big-endian `uint64` content length, and the UTF-8 encoded content bytes,
terminated by a `uint32` of `0`.

## Domain types

All domain models are exported from the root barrel:

| Module              | Types                                                                         |
| :------------------ | :---------------------------------------------------------------------------- |
| `project`           | `ProjectId`, `ScriptId`, `FileId`, `InstanceId`                               |
| `context`           | `ContextSlice`, `ContextGraphNode`, `ContextGraphEdge`, `ContextGraphResult`  |
| `analysis`          | `AnalysisId`, `AnalysisStatus`, `AnalysisInfo`                                |
| `code-generation`   | `SampleCodeFile`                                                              |
| `plugins`           | `PluginId`, `PluginKind`, `PluginManifest`, `Plugin`, `PluginExtensionPoint`  |

## Results SDK

The `DefaultAnalysisResultService` lets analysis logic emit typed results (files, console
output, errors, or custom types) without managing file IDs, byte encoding, or low-level storage.

### Storage Decoupling

The SDK deliberately does not provide any in-memory or built-in storage implementation. The SDK only defines the basic abstractions — concrete storage is something that the concrete implementer offers by implementing `ScriptResultsStore` (backed by e.g. S3, disk, blob storage, or a database).


### Interfaces

```typescript
export interface ScriptResultsStore {
  uploadFile(fileId: string, content: Uint8Array, metadata?: Record<string, string>): Promise<void>;
}

export interface AnalysisResultService {
  addFile(name: string, content: string | Uint8Array, resultType?: string): Promise<ResultId>;
  addConsole(content: string, name?: string): Promise<ResultId>;
  addError(message: string, name?: string): Promise<ResultId>;
  addResult(name: string, resultType: string, content: string | Uint8Array): Promise<ResultId>;
}
```

### Usage

```typescript
import {
  DefaultAnalysisResultService,
  ResultType,
  type ScriptResultsStore,
} from "@dxworks/scriptbee-analysis-sdk";

class FileSystemResultsStore implements ScriptResultsStore {
  async uploadFile(fileId: string, content: Uint8Array, metadata?: Record<string, string>): Promise<void> {
    // concrete storage implementation (e.g. S3, disk, blob storage)
  }
}

const store = new FileSystemResultsStore();
const results = new DefaultAnalysisResultService({ store });

// emit a file result
const resultId = await results.addFile("summary.json", '{"count": 42}');

// emit console output
await results.addConsole("Analysis started");

// emit an error
await results.addError("Script timed out");

// emit a custom result type
await results.addResult("chart.svg", "Chart", svgBytes);
```

### Result types

Built-in type constants:

| Constant               | Value        |
| :--------------------- | :----------- |
| `ResultType.FILE`      | `"File"`     |
| `ResultType.CONSOLE`   | `"Console"`  |
| `ResultType.RUN_ERROR` | `"RunError"` |

### Callback hook

`DefaultAnalysisResultService` accepts an optional `onResultAdded` callback in its constructor options:

```typescript
const summaries: ResultSummary[] = [];
const results = new DefaultAnalysisResultService({
  store,
  onResultAdded: (summary) => summaries.push(summary),
});
```

## Versioning & Changelog

The SDK follows [Semantic Versioning](https://semver.org/). Releases are tagged `@dxworks/scriptbee-analysis-sdk@<version>`.
See the shared [CHANGELOG.md](https://github.com/dxworks/scriptbee/blob/main/Integrations/sdks/CHANGELOG.md) for details.
