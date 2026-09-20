from typing import Protocol

from fastapi import APIRouter

from scriptbee_analysis_sdk._internal.endpoints.analyses_router import make_analyses_router
from scriptbee_analysis_sdk._internal.endpoints.context_router import make_context_router
from scriptbee_analysis_sdk._internal.endpoints.plugins_router import make_plugins_router
from scriptbee_analysis_sdk.abstractions.clear_context_use_case import ClearContextUseCase
from scriptbee_analysis_sdk.abstractions.generate_classes_use_case import GenerateClassesUseCase
from scriptbee_analysis_sdk.abstractions.get_context_graph_use_case import GetContextGraphUseCase
from scriptbee_analysis_sdk.abstractions.get_context_use_case import GetContextUseCase
from scriptbee_analysis_sdk.abstractions.get_installed_plugins_use_case import (
    GetInstalledPluginsUseCase,
)
from scriptbee_analysis_sdk.abstractions.install_plugin_use_case import InstallPluginUseCase
from scriptbee_analysis_sdk.abstractions.link_context_use_case import LinkContextUseCase
from scriptbee_analysis_sdk.abstractions.load_context_use_case import LoadContextUseCase
from scriptbee_analysis_sdk.abstractions.run_analysis_use_case import RunAnalysisUseCase
from scriptbee_analysis_sdk.abstractions.uninstall_plugin_use_case import UninstallPluginUseCase


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


def create_analysis_sdk_router(container: AnalysisSdkContainer) -> APIRouter:
    router = APIRouter()

    router.include_router(make_analyses_router(lambda: container.run_analysis))
    router.include_router(
        make_context_router(
            get_context=lambda: container.get_context,
            load_context=lambda: container.load_context,
            link_context=lambda: container.link_context,
            clear_context=lambda: container.clear_context,
            get_context_graph=lambda: container.get_context_graph,
            generate_classes=lambda: container.generate_classes,
        )
    )
    router.include_router(
        make_plugins_router(
            get_installed_plugins=lambda: container.get_installed_plugins,
            install_plugin=lambda: container.install_plugin,
            uninstall_plugin=lambda: container.uninstall_plugin,
        )
    )

    return router
