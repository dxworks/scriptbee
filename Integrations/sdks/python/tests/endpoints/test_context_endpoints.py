import io
import struct

from httpx import ASGITransport, AsyncClient

from scriptbee_analysis_sdk.abstractions import (
    ClearContextUseCase,
    GenerateClassesUseCase,
    GetContextGraphUseCase,
    GetContextUseCase,
    LinkContextUseCase,
    LoadContextUseCase,
)
from scriptbee_analysis_sdk.domain.code_generation import SampleCodeFile
from scriptbee_analysis_sdk.domain.context import (
    ContextGraphEdge,
    ContextGraphNode,
    ContextGraphResult,
    ContextSlice,
)
from scriptbee_analysis_sdk.domain.project import FileId
from tests.endpoints.conftest import StubContainer, make_app


class GetContextStub(GetContextUseCase):
    def __init__(self, slices):
        self._slices = slices

    def get(self):
        return self._slices


class LoadContextStub(LoadContextUseCase):
    def __init__(self):
        self.received = None

    async def load(self, files_to_load):
        self.received = files_to_load


class LinkContextStub(LinkContextUseCase):
    def __init__(self):
        self.received = None

    async def link(self, linker_ids):
        self.received = linker_ids


class ClearContextStub(ClearContextUseCase):
    def __init__(self):
        self.called = False

    def clear(self):
        self.called = True


class GetContextGraphStub(GetContextGraphUseCase):
    def __init__(self, result):
        self._result = result
        self.received_query = None
        self.received_node_id = None

    def search_nodes(self, query, offset, limit):
        self.received_query = query
        return self._result

    def get_neighbors(self, node_id):
        self.received_node_id = node_id
        return self._result


class GenerateClassesStub(GenerateClassesUseCase):
    def __init__(self, files):
        self._files = files
        self.received_languages = None

    async def generate_classes(self, languages):
        self.received_languages = languages
        return self._files


async def test_get_context_returns_slices():
    slices = [ContextSlice(model="MyModel", plugin_ids=["loader-1"])]
    app = make_app(StubContainer(get_context=GetContextStub(slices)))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.get("/api/context")

    assert response.status_code == 200
    body = response.json()
    assert body["data"][0]["model"] == "MyModel"
    assert body["data"][0]["plugin_ids"] == ["loader-1"]


async def test_load_context_returns_204():
    stub = LoadContextStub()
    app = make_app(StubContainer(load_context=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post(
            "/api/context/load",
            json={"files_to_load": {"loader-1": ["file-a", "file-b"]}},
        )

    assert response.status_code == 204
    assert isinstance(stub.received["loader-1"][0], FileId)


async def test_load_context_returns_422_when_files_to_load_missing():
    app = make_app(StubContainer(load_context=LoadContextStub()))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/context/load", json={})

    assert response.status_code == 422


async def test_link_context_returns_204():
    stub = LinkContextStub()
    app = make_app(StubContainer(link_context=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/context/link", json={"linker_ids": ["linker-1"]})

    assert response.status_code == 204
    assert stub.received == ["linker-1"]


async def test_link_context_returns_422_when_linker_ids_missing():
    app = make_app(StubContainer(link_context=LinkContextStub()))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/context/link", json={})

    assert response.status_code == 422


async def test_clear_context_returns_204():
    stub = ClearContextStub()
    app = make_app(StubContainer(clear_context=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/context/clear")

    assert response.status_code == 204
    assert stub.called


async def test_search_nodes_returns_graph_response():
    result = ContextGraphResult(
        nodes=[ContextGraphNode(id="n1", label="A", type="Class")],
        edges=[ContextGraphEdge(source="n1", target="n1", label="self")],
    )
    stub = GetContextGraphStub(result)
    app = make_app(StubContainer(get_context_graph=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.get("/api/context/graph-nodes?query=A&offset=0&limit=10")

    assert response.status_code == 200
    body = response.json()
    assert body["nodes"][0]["id"] == "n1"
    assert body["edges"][0]["label"] == "self"
    assert stub.received_query == "A"


async def test_get_neighbors_returns_graph_response():
    result = ContextGraphResult(nodes=[], edges=[])
    stub = GetContextGraphStub(result)
    app = make_app(StubContainer(get_context_graph=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.get("/api/context/graph-nodes/node-42/neighbors")

    assert response.status_code == 200
    assert stub.received_node_id == "node-42"


async def test_generate_classes_returns_binary_stream():
    files = [SampleCodeFile(name="Model.cs", content="class Model {}")]
    stub = GenerateClassesStub(files)
    app = make_app(StubContainer(generate_classes=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post(
            "/api/context/generate-classes",
            json={"languages": ["csharp"]},
        )

    assert response.status_code == 200
    assert "application/octet-stream" in response.headers["content-type"]
    assert "classes.bin" in response.headers["content-disposition"]
    assert stub.received_languages == ["csharp"]

    stream = io.BytesIO(response.content)
    (path_len,) = struct.unpack(">I", stream.read(4))
    assert path_len > 0


async def test_generate_classes_uses_empty_list_when_languages_omitted():
    stub = GenerateClassesStub([])
    app = make_app(StubContainer(generate_classes=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/context/generate-classes", json={})

    assert response.status_code == 200
    assert stub.received_languages == []
