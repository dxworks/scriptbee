from fastapi import APIRouter, Depends
from fastapi.responses import JSONResponse, Response

from scriptbee_analysis_sdk._internal.endpoints.contracts.web_models import (
    WebGetInstalledPluginsResponse,
    WebInstalledPlugin,
    WebInstallPluginCommand,
)
from scriptbee_analysis_sdk.abstractions.errors.invalid_plugin_error import InvalidPluginError
from scriptbee_analysis_sdk.abstractions.get_installed_plugins_use_case import (
    GetInstalledPluginsUseCase,
)
from scriptbee_analysis_sdk.abstractions.install_plugin_use_case import (
    InstallPluginUseCase,
    Success,
)
from scriptbee_analysis_sdk.abstractions.uninstall_plugin_use_case import UninstallPluginUseCase
from scriptbee_analysis_sdk.domain.plugins import PluginId


def make_plugins_router(
    get_installed_plugins: callable,
    install_plugin: callable,
    uninstall_plugin: callable,
) -> APIRouter:
    router = APIRouter(tags=["Plugins"])

    @router.get(
        "/api/plugins",
        name="PluginsGet",
        summary="Get installed plugins",
        description="Retrieves a list of all plugins currently installed in the analysis service.",
    )
    def get_installed_plugins_handler(
        use_case: GetInstalledPluginsUseCase = Depends(get_installed_plugins),
    ) -> WebGetInstalledPluginsResponse:
        plugins = use_case.get()
        return WebGetInstalledPluginsResponse(
            data=[WebInstalledPlugin.from_plugin(p) for p in plugins]
        )

    @router.post(
        "/api/plugins",
        name="PluginsPost",
        summary="Install a plugin",
        description="Installs a specific plugin version into the analysis service.",
    )
    def install_plugin_handler(
        command: WebInstallPluginCommand,
        use_case: InstallPluginUseCase = Depends(install_plugin),
    ):
        plugin_id = PluginId(name=command.plugin_id, version=command.version)
        result = use_case.install_plugin(plugin_id)

        if isinstance(result, Success):
            return Response(status_code=204)
        if isinstance(result, InvalidPluginError):
            return JSONResponse(
                status_code=400,
                content={
                    "title": "Plugin Installation Failed",
                    "detail": f"Invalid plugin version: {result.id.version} for {result.id.name}",
                },
            )
        return JSONResponse(
            status_code=500,
            content={
                "title": "Plugin Installation Failed",
                "detail": f"An error occurred while installing plugin {result.id.name} version {result.id.version}",
            },
        )

    @router.delete(
        "/api/plugins/{plugin_id}",
        status_code=204,
        name="PluginsDelete",
        summary="Uninstall a plugin",
        description="Uninstalls a specific plugin version from the analysis service.",
    )
    def uninstall_plugin_handler(
        plugin_id: str,
        version: str,
        use_case: UninstallPluginUseCase = Depends(uninstall_plugin),
    ) -> None:
        use_case.uninstall_plugin(PluginId(name=plugin_id, version=version))

    return router
