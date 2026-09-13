from datetime import UTC, datetime

from fastapi import FastAPI

from scriptbee_analysis_sdk.abstractions import (
    ClearContextUseCase,
    GenerateClassesUseCase,
    GetContextGraphUseCase,
    GetContextUseCase,
    GetInstalledPluginsUseCase,
    InstallPluginUseCase,
    LinkContextUseCase,
    LoadContextUseCase,
    RunAnalysisUseCase,
    UninstallPluginUseCase,
)
from scriptbee_analysis_sdk.domain.analysis import AnalysisId, AnalysisInfo, AnalysisStatus
from scriptbee_analysis_sdk.domain.project import InstanceId, ProjectId, ScriptId
from scriptbee_analysis_sdk.extensions import create_analysis_sdk_router


def make_analysis_info(project_id="proj-1", script_id="script-1") -> AnalysisInfo:
    return AnalysisInfo(
        id=AnalysisId(value="analysis-id-1"),
        project_id=ProjectId(value=project_id),
        instance_id=InstanceId(value="inst-1"),
        script_id=ScriptId(value=script_id),
        status=AnalysisStatus(value="Started"),
        creation_date=datetime(2024, 1, 1, tzinfo=UTC),
    )


class _Unset:
    pass


_UNSET = _Unset()


class StubContainer:
    def __init__(self, **overrides):
        self._overrides = overrides

    def _get(self, name):
        val = self._overrides.get(name, _UNSET)
        if isinstance(val, _Unset):
            raise RuntimeError(f"Use case '{name}' not configured in test")
        return val

    @property
    def run_analysis(self) -> RunAnalysisUseCase:
        return self._get("run_analysis")

    @property
    def get_context(self) -> GetContextUseCase:
        return self._get("get_context")

    @property
    def load_context(self) -> LoadContextUseCase:
        return self._get("load_context")

    @property
    def link_context(self) -> LinkContextUseCase:
        return self._get("link_context")

    @property
    def clear_context(self) -> ClearContextUseCase:
        return self._get("clear_context")

    @property
    def get_context_graph(self) -> GetContextGraphUseCase:
        return self._get("get_context_graph")

    @property
    def generate_classes(self) -> GenerateClassesUseCase:
        return self._get("generate_classes")

    @property
    def get_installed_plugins(self) -> GetInstalledPluginsUseCase:
        return self._get("get_installed_plugins")

    @property
    def install_plugin(self) -> InstallPluginUseCase:
        return self._get("install_plugin")

    @property
    def uninstall_plugin(self) -> UninstallPluginUseCase:
        return self._get("uninstall_plugin")


def make_app(container: StubContainer) -> FastAPI:
    app = FastAPI()
    app.include_router(create_analysis_sdk_router(container))
    return app
