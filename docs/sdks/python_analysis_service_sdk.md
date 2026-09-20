# Python Analysis Service SDK

The `scriptbee-analysis-sdk` package is the Python implementation of the
[Analysis Service SDK](../index). It lets you build a fully-featured Analysis Service in Python
using [FastAPI](https://fastapi.tiangolo.com/) without writing any HTTP boilerplate.

## Installation

```bash
pip install scriptbee-analysis-sdk
```

Or with [uv](https://docs.astral.sh/uv/):

```bash
uv add scriptbee-analysis-sdk
```

## What the SDK provides

| Layer            | What's included                                                            |
| :--------------- | :------------------------------------------------------------------------- |
| **Endpoints**    | All REST routes pre-wired with correct HTTP verbs, paths, and status codes |
| **Contracts**    | `Web*` Pydantic request/response models matching the public REST API       |
| **Validators**   | Pydantic field validators for all request models                           |
| **Abstractions** | Abstract base classes your service must implement                          |
| **Utilities**    | `FileBundler` for packaging file streams for loader plugins                |
| **Registration** | `create_analysis_sdk_router()` to mount everything in one call             |

## Architecture

```
┌────────────────┐   REST API    ┌────────────────────────────────────┐
│   Gateway      │ ─────────────▶│   Your FastAPI Analysis Service    │
│ (ScriptBee)    │               │                                    │
└────────────────┘               │  create_analysis_sdk_router()      │
                                 │  ┌────────────────────────────┐    │
                                 │  │  Your AnalysisSdkContainer  │    │
                                 │  │  (10 use-case properties)   │    │
                                 │  └────────────────────────────┘    │
                                 └────────────────────────────────────┘
```

## Implementing the use cases

Implement each abstract base class from `scriptbee_analysis_sdk.abstractions` and expose them through
an `AnalysisSdkContainer`-compatible object.

| Abstract base class          | Responsibility                              |
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

```python
from scriptbee_analysis_sdk.abstractions import RunAnalysisUseCase, RunAnalysisCommand
from scriptbee_analysis_sdk.domain.analysis import AnalysisInfo


class Neo4jRunAnalysisService(RunAnalysisUseCase):
    def __init__(self, session):
        self._session = session

    async def run(self, command: RunAnalysisCommand) -> AnalysisInfo:
        # store results in Neo4j, return analysis info
        ...
```

## Registration

Create a container that satisfies the `AnalysisSdkContainer` protocol, then call
`create_analysis_sdk_router()` to get a pre-configured `APIRouter`:

```python
from fastapi import FastAPI
from scriptbee_analysis_sdk.extensions import AnalysisSdkContainer, create_analysis_sdk_router


class MyContainer:
    @property
    def run_analysis(self):
        return Neo4jRunAnalysisService(session=...)

    @property
    def get_context(self):
        return Neo4jGetContextService(...)

    # ... all other use-case properties


app = FastAPI()
app.include_router(create_analysis_sdk_router(MyContainer()))
```

`create_analysis_sdk_router()` registers all endpoints under `/api/` and wires each route to the
corresponding use case resolved lazily from the container.

## Container protocol

The `AnalysisSdkContainer` is a structural `Protocol` — any object with the right properties satisfies
it without inheritance:

```python
class AnalysisSdkContainer(Protocol):
    @property
    def run_analysis(self) -> RunAnalysisUseCase: ...
    @property
    def get_context(self) -> GetContextUseCase: ...
    @property
    def load_context(self) -> LoadContextUseCase: ...
    @property
    def link_context(self) -> LinkContextUseCase: ...
    @property
    def clear_context(self) -> ClearContextUseCase: ...
    @property
    def get_context_graph(self) -> GetContextGraphUseCase: ...
    @property
    def generate_classes(self) -> GenerateClassesUseCase: ...
    @property
    def get_installed_plugins(self) -> GetInstalledPluginsUseCase: ...
    @property
    def install_plugin(self) -> InstallPluginUseCase: ...
    @property
    def uninstall_plugin(self) -> UninstallPluginUseCase: ...
```

## FileBundler

The `FileBundler` utility serialises a list of `SampleCodeFile` objects into the binary format
consumed by the Gateway for the generate-classes endpoint.

```python
import io
from scriptbee_analysis_sdk.file_bundler import FileBundler
from scriptbee_analysis_sdk.domain.code_generation import SampleCodeFile

files = [SampleCodeFile(name="Model.cs", content="public class Model {}")]

stream = io.BytesIO()
FileBundler().write_to_stream(files, stream)
stream.seek(0)
# stream now contains the binary-encoded file bundle
```

The wire format is: for each file, a big-endian `uint32` path length, the UTF-8 encoded path, a
big-endian `uint32` content length, and the UTF-8 encoded content. A terminal `uint32` of `0`
signals end-of-stream.

## Domain types

All domain types live under `scriptbee_analysis_sdk.domain` and are Pydantic `BaseModel`s.

| Module            | Types                                                                         |
| :---------------- | :---------------------------------------------------------------------------- |
| `project`         | `ProjectId`, `ScriptId`, `FileId`, `InstanceId`                               |
| `context`         | `ContextSlice`, `ContextGraphNode`, `ContextGraphEdge`, `ContextGraphResult`  |
| `analysis`        | `AnalysisId`, `AnalysisStatus`, `AnalysisInfo`                                |
| `code_generation` | `SampleCodeFile`                                                              |
| `plugins`         | `PluginId`, `PluginKind`, `PluginManifest`, `Plugin`, `*PluginExtensionPoint` |
| `scripts`         | `Script`, `ScriptLanguage`                                                    |

## Results SDK

The `DefaultAnalysisResultService` lets your analysis logic emit typed results (files, console
output, errors, or custom types) without managing file IDs, byte encoding, or store calls directly.

### Interface

```python
class AnalysisResultService(Protocol):
    async def add_file(
        self,
        name: str,
        content: str | bytes,
        result_type: str = ResultType.FILE,
    ) -> ResultId: ...

    async def add_console(self, content: str, name: str = "ConsoleOutput") -> ResultId: ...

    async def add_error(self, message: str, name: str = "RunError") -> ResultId: ...

    async def add_result(self, name: str, result_type: str, content: str | bytes) -> ResultId: ...
```

### Storage Decoupling

The SDK deliberately does not provide any in-memory or built-in storage implementation. The SDK only offers the basic abstractions — storage is something that the concrete implementer offers by implementing the `ScriptResultsStore` protocol.

```python
class ScriptResultsStore(Protocol):
    async def upload_file(
        self,
        file_id: str,
        content: bytes,
        metadata: dict[str, str] | None = None,
    ) -> None: ...
```

### Usage

```python
from scriptbee_analysis_sdk.domain import ResultType
from scriptbee_analysis_sdk.results import DefaultAnalysisResultService, ScriptResultsStore


class FileSystemScriptResultsStore:
    async def upload_file(
        self,
        file_id: str,
        content: bytes,
        metadata: dict[str, str] | None = None,
    ) -> None:
        # concrete storage implementation (e.g. S3, disk, blob storage)
        ...


store = FileSystemScriptResultsStore()
results = DefaultAnalysisResultService(store=store)

# emit a text file result
result_id = await results.add_file("summary.json", '{"count": 42}')

# emit console output
await results.add_console("Analysis started")

# emit an error
await results.add_error("Script timed out")

# emit a custom result type
await results.add_result("chart.svg", "Chart", svg_bytes)
```

### Result types

The built-in type constants are on `ResultType`:

| Constant               | Value        |
| :--------------------- | :----------- |
| `ResultType.FILE`      | `"File"`     |
| `ResultType.CONSOLE`   | `"Console"`  |
| `ResultType.RUN_ERROR` | `"RunError"` |

You can pass any custom string as `result_type` in `add_file` or `add_result` to define your own
result categories.

### Callback hook

`DefaultAnalysisResultService` accepts an optional `on_result_added` callback that is invoked
with a `ResultSummary` after each result is persisted. The host layer uses this to track results
without leaking infrastructure concerns into the analysis logic:

```python
summaries = []
results = DefaultAnalysisResultService(
    store=store,
    on_result_added=summaries.append,
)
```

## Versioning & Changelog

The SDK follows [Semantic Versioning](https://semver.org/). Releases are tagged `analysis-sdk-python@<version>`
in the monorepo and published automatically to PyPI.

See the shared [CHANGELOG.md](https://github.com/dxworks/scriptbee/blob/main/integrations/sdks/CHANGELOG.md)
for the full release history of all SDK implementations.
