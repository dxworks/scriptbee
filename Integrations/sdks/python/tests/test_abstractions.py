from datetime import UTC, datetime

import pytest

from scriptbee_analysis_sdk.abstractions import (
    ClearContextUseCase,
    GenerateClassesUseCase,
    GetContextGraphUseCase,
    GetContextUseCase,
    GetInstalledPluginsUseCase,
    InstallPluginUseCase,
    InvalidPluginError,
    LinkContextUseCase,
    LoadContextUseCase,
    PluginInstallationError,
    RunAnalysisCommand,
    RunAnalysisUseCase,
    Success,
    UninstallPluginUseCase,
)
from scriptbee_analysis_sdk.domain.analysis import AnalysisId, AnalysisInfo, AnalysisStatus
from scriptbee_analysis_sdk.domain.code_generation import SampleCodeFile
from scriptbee_analysis_sdk.domain.context import ContextGraphResult, ContextSlice
from scriptbee_analysis_sdk.domain.plugins import Plugin, PluginId
from scriptbee_analysis_sdk.domain.project import FileId, InstanceId, ProjectId, ScriptId


class StubClearContext(ClearContextUseCase):
    def clear(self) -> None:
        pass


class StubGetContext(GetContextUseCase):
    def get(self) -> list[ContextSlice]:
        return []


class StubLoadContext(LoadContextUseCase):
    async def load(self, files_to_load: dict[str, list[FileId]]) -> None:
        pass


class StubLinkContext(LinkContextUseCase):
    async def link(self, linker_ids: list[str]) -> None:
        pass


class StubGetContextGraph(GetContextGraphUseCase):
    def search_nodes(self, query: str, offset: int, limit: int) -> ContextGraphResult:
        return ContextGraphResult(nodes=[], edges=[])

    def get_neighbors(self, node_id: str) -> ContextGraphResult:
        return ContextGraphResult(nodes=[], edges=[])


class StubGenerateClasses(GenerateClassesUseCase):
    async def generate_classes(self, languages: list[str]) -> list[SampleCodeFile]:
        return []


class StubRunAnalysis(RunAnalysisUseCase):
    async def run(self, command: RunAnalysisCommand) -> AnalysisInfo:
        return AnalysisInfo(
            id=AnalysisId(value="id-1"),
            project_id=command.project_id,
            instance_id=InstanceId(value="inst-1"),
            script_id=command.script_id,
            status=AnalysisStatus(value="Started"),
            creation_date=datetime(2024, 1, 1, tzinfo=UTC),
        )


class StubGetInstalledPlugins(GetInstalledPluginsUseCase):
    def get(self) -> list[Plugin]:
        return []


class StubInstallPlugin(InstallPluginUseCase):
    def __init__(self, result):
        self._result = result

    def install_plugin(self, plugin_id: PluginId):
        return self._result


class StubUninstallPlugin(UninstallPluginUseCase):
    def uninstall_plugin(self, plugin_id: PluginId) -> None:
        pass


def test_clear_context_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        ClearContextUseCase()  # type: ignore[abstract]


def test_get_context_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        GetContextUseCase()  # type: ignore[abstract]


def test_load_context_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        LoadContextUseCase()  # type: ignore[abstract]


def test_link_context_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        LinkContextUseCase()  # type: ignore[abstract]


def test_get_context_graph_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        GetContextGraphUseCase()  # type: ignore[abstract]


def test_generate_classes_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        GenerateClassesUseCase()  # type: ignore[abstract]


def test_run_analysis_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        RunAnalysisUseCase()  # type: ignore[abstract]


def test_get_installed_plugins_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        GetInstalledPluginsUseCase()  # type: ignore[abstract]


def test_install_plugin_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        InstallPluginUseCase()  # type: ignore[abstract]


def test_uninstall_plugin_cannot_be_instantiated_without_implementation():
    with pytest.raises(TypeError):
        UninstallPluginUseCase()  # type: ignore[abstract]


def test_clear_context_stub_calls_clear():
    StubClearContext().clear()


def test_get_context_stub_returns_list():
    assert StubGetContext().get() == []


async def test_load_context_stub_accepts_files():
    await StubLoadContext().load({"loader-1": [FileId(value="f1")]})


async def test_link_context_stub_accepts_linker_ids():
    await StubLinkContext().link(["linker-1"])


def test_get_context_graph_search_nodes():
    result = StubGetContextGraph().search_nodes("q", 0, 10)
    assert result.nodes == []


def test_get_context_graph_get_neighbors():
    result = StubGetContextGraph().get_neighbors("node-1")
    assert result.nodes == []


async def test_generate_classes_stub_returns_list():
    result = await StubGenerateClasses().generate_classes(["csharp"])
    assert result == []


async def test_run_analysis_stub_returns_analysis_info():
    command = RunAnalysisCommand(
        project_id=ProjectId(value="proj-1"),
        script_id=ScriptId(value="script-1"),
    )
    result = await StubRunAnalysis().run(command)
    assert result.id.value == "id-1"
    assert result.status.value == "Started"


def test_get_installed_plugins_stub_returns_list():
    assert StubGetInstalledPlugins().get() == []


def test_install_plugin_returns_success():
    assert isinstance(
        StubInstallPlugin(Success()).install_plugin(PluginId(name="p", version="1.0")), Success
    )


def test_install_plugin_returns_invalid_error():
    pid = PluginId(name="p", version="bad")
    result = StubInstallPlugin(InvalidPluginError(id=pid)).install_plugin(pid)
    assert isinstance(result, InvalidPluginError)
    assert result.id == pid


def test_install_plugin_returns_installation_error():
    pid = PluginId(name="p", version="1.0")
    result = StubInstallPlugin(PluginInstallationError(id=pid)).install_plugin(pid)
    assert isinstance(result, PluginInstallationError)


def test_uninstall_plugin_stub_calls_method():
    StubUninstallPlugin().uninstall_plugin(PluginId(name="p", version="1.0"))


def test_run_analysis_command_stores_ids():
    cmd = RunAnalysisCommand(
        project_id=ProjectId(value="proj-1"),
        script_id=ScriptId(value="script-1"),
    )
    assert cmd.project_id.value == "proj-1"
    assert cmd.script_id.value == "script-1"


def test_run_analysis_command_is_immutable():
    cmd = RunAnalysisCommand(
        project_id=ProjectId(value="p"),
        script_id=ScriptId(value="s"),
    )
    with pytest.raises((AttributeError, TypeError)):
        cmd.project_id = ProjectId(value="other")  # type: ignore[misc]


def test_invalid_plugin_error_stores_id():
    pid = PluginId(name="bad-plugin", version="9.9.9")
    assert InvalidPluginError(id=pid).id == pid


def test_plugin_installation_error_stores_id():
    pid = PluginId(name="p", version="1.0")
    assert PluginInstallationError(id=pid).id == pid


def test_success_is_distinct_from_errors():
    s = Success()
    assert not isinstance(s, InvalidPluginError)
    assert not isinstance(s, PluginInstallationError)
