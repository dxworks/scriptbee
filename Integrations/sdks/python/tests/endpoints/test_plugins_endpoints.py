from httpx import ASGITransport, AsyncClient

from scriptbee_analysis_sdk.abstractions import (
    GetInstalledPluginsUseCase,
    InstallPluginUseCase,
    Success,
    UninstallPluginUseCase,
)
from scriptbee_analysis_sdk.abstractions.errors import InvalidPluginError, PluginInstallationError
from scriptbee_analysis_sdk.domain.plugins import (
    LoaderPluginExtensionPoint,
    Plugin,
    PluginId,
    PluginManifest,
)
from tests.endpoints.conftest import StubContainer, make_app


def make_plugin(name="my-plugin", version="1.0.0"):
    return Plugin(
        folder_path=f"/plugins/{name}",
        id=PluginId(name=name, version=version),
        manifest=PluginManifest(
            api_version="1.0",
            name=name,
            extension_points=[
                LoaderPluginExtensionPoint(entry_point="loader.dll", version="1.0.0")
            ],
        ),
    )


class GetInstalledPluginsStub(GetInstalledPluginsUseCase):
    def __init__(self, plugins):
        self._plugins = plugins

    def get(self):
        return self._plugins


class InstallPluginStub(InstallPluginUseCase):
    def __init__(self, result):
        self._result = result
        self.received_id = None

    def install_plugin(self, plugin_id):
        self.received_id = plugin_id
        return self._result


class UninstallPluginStub(UninstallPluginUseCase):
    def __init__(self):
        self.received_id = None

    def uninstall_plugin(self, plugin_id):
        self.received_id = plugin_id


async def test_get_installed_plugins_returns_list():
    stub = GetInstalledPluginsStub([make_plugin()])
    app = make_app(StubContainer(get_installed_plugins=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.get("/api/plugins")

    assert response.status_code == 200
    body = response.json()
    assert len(body["data"]) == 1
    assert body["data"][0]["id"] == "my-plugin"
    assert body["data"][0]["version"] == "1.0.0"


async def test_install_plugin_returns_204_on_success():
    stub = InstallPluginStub(Success())
    app = make_app(StubContainer(install_plugin=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/plugins", json={"plugin_id": "my-plugin", "version": "1.0.0"})

    assert response.status_code == 204
    assert stub.received_id == PluginId(name="my-plugin", version="1.0.0")


async def test_install_plugin_returns_400_on_invalid_plugin_error():
    plugin_id = PluginId(name="bad-plugin", version="9.9.9")
    stub = InstallPluginStub(InvalidPluginError(id=plugin_id))
    app = make_app(StubContainer(install_plugin=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post(
            "/api/plugins", json={"plugin_id": "bad-plugin", "version": "9.9.9"}
        )

    assert response.status_code == 400
    assert "bad-plugin" in response.json()["detail"]


async def test_install_plugin_returns_500_on_installation_error():
    plugin_id = PluginId(name="failing-plugin", version="1.0.0")
    stub = InstallPluginStub(PluginInstallationError(id=plugin_id))
    app = make_app(StubContainer(install_plugin=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post(
            "/api/plugins", json={"plugin_id": "failing-plugin", "version": "1.0.0"}
        )

    assert response.status_code == 500
    assert "failing-plugin" in response.json()["detail"]


async def test_install_plugin_returns_422_on_empty_plugin_id():
    app = make_app(StubContainer(install_plugin=InstallPluginStub(Success())))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/plugins", json={"plugin_id": "", "version": "1.0.0"})

    assert response.status_code == 422


async def test_install_plugin_returns_422_on_empty_version():
    app = make_app(StubContainer(install_plugin=InstallPluginStub(Success())))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/plugins", json={"plugin_id": "plugin", "version": ""})

    assert response.status_code == 422


async def test_uninstall_plugin_returns_204():
    stub = UninstallPluginStub()
    app = make_app(StubContainer(uninstall_plugin=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.delete("/api/plugins/my-plugin?version=1.0.0")

    assert response.status_code == 204
    assert stub.received_id.name == "my-plugin"
    assert stub.received_id.version == "1.0.0"


def collect_routes(app) -> set[str]:
    from fastapi.routing import APIRoute

    paths = set()

    def _walk(routes):
        for route in routes:
            if isinstance(route, APIRoute):
                paths.add(route.path)
            # FastAPI >= 0.115 wraps included routers as _IncludedRouter
            if hasattr(route, "original_router"):
                _walk(route.original_router.routes)
            elif hasattr(route, "routes"):
                _walk(route.routes)

    _walk(app.router.routes)
    return paths


def test_sdk_router_registers_all_routes():
    app = make_app(StubContainer())
    routes = collect_routes(app)

    assert "/api/analyses" in routes
    assert "/api/context" in routes
    assert "/api/context/load" in routes
    assert "/api/context/link" in routes
    assert "/api/context/clear" in routes
    assert "/api/context/graph-nodes" in routes
    assert "/api/context/graph-nodes/{node_id}/neighbors" in routes
    assert "/api/context/generate-classes" in routes
    assert "/api/plugins" in routes
    assert "/api/plugins/{plugin_id}" in routes
