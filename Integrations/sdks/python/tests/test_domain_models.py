"""Tests for domain model types."""

from datetime import UTC, datetime

import pytest

from scriptbee_analysis_sdk.domain.analysis import AnalysisId, AnalysisInfo, AnalysisStatus
from scriptbee_analysis_sdk.domain.code_generation import SampleCodeFile
from scriptbee_analysis_sdk.domain.context import (
    ContextGraphEdge,
    ContextGraphNode,
    ContextGraphResult,
    ContextSlice,
)
from scriptbee_analysis_sdk.domain.plugins import (
    FilePreviewerOutlet,
    HelperFunctionsPluginExtensionPoint,
    LinkerPluginExtensionPoint,
    LoaderPluginExtensionPoint,
    NestedPluginExtensionPoint,
    OutletType,
    Plugin,
    PluginId,
    PluginKind,
    PluginManifest,
    ScriptGeneratorPluginExtensionPoint,
    ScriptRunnerPluginExtensionPoint,
    SidePanelOutlet,
    TopNavigationBarOutlet,
    UiPluginExtensionPoint,
)
from scriptbee_analysis_sdk.domain.project import FileId, InstanceId, ProjectId, ScriptId
from scriptbee_analysis_sdk.domain.scripts import Script, ScriptLanguage

# ---------------------------------------------------------------------------
# ProjectId / ScriptId / FileId
# ---------------------------------------------------------------------------


def test_project_id_stores_value():
    pid = ProjectId(value="my-project")
    assert pid.value == "my-project"
    assert str(pid) == "my-project"


def test_project_id_rejects_empty():
    with pytest.raises(Exception):
        ProjectId(value="")


def test_project_id_rejects_whitespace():
    with pytest.raises(Exception):
        ProjectId(value="   ")


def test_script_id_stores_value():
    sid = ScriptId(value="script-123")
    assert str(sid) == "script-123"


def test_script_id_rejects_empty():
    with pytest.raises(Exception):
        ScriptId(value="")


def test_file_id_stores_value():
    fid = FileId(value="file-abc")
    assert str(fid) == "file-abc"


def test_file_id_rejects_empty():
    with pytest.raises(Exception):
        FileId(value="")


def test_instance_id_stores_value():
    iid = InstanceId(value="instance-xyz")
    assert str(iid) == "instance-xyz"


# ---------------------------------------------------------------------------
# ContextSlice
# ---------------------------------------------------------------------------


def test_context_slice_stores_fields():
    cs = ContextSlice(model="MyModel", plugin_ids=["loader-1", "loader-2"])
    assert cs.model == "MyModel"
    assert cs.plugin_ids == ["loader-1", "loader-2"]


def test_context_slice_empty_plugin_ids():
    cs = ContextSlice(model="Model", plugin_ids=[])
    assert cs.plugin_ids == []


# ---------------------------------------------------------------------------
# ContextGraphNode / Edge / Result
# ---------------------------------------------------------------------------


def test_context_graph_node_stores_fields():
    node = ContextGraphNode(
        id="n1",
        label="MyClass",
        type="Class",
        loader="loader-1",
        properties={"key": "value"},
    )
    assert node.id == "n1"
    assert node.loader == "loader-1"
    assert node.properties == {"key": "value"}


def test_context_graph_node_optional_loader():
    node = ContextGraphNode(id="n2", label="X", type="T")
    assert node.loader is None
    assert node.properties == {}


def test_context_graph_edge_stores_fields():
    edge = ContextGraphEdge(source="n1", target="n2", label="calls")
    assert edge.source == "n1"
    assert edge.target == "n2"
    assert edge.label == "calls"


def test_context_graph_result_stores_nodes_and_edges():
    result = ContextGraphResult(
        nodes=[ContextGraphNode(id="n1", label="A", type="T")],
        edges=[ContextGraphEdge(source="n1", target="n1", label="self")],
    )
    assert len(result.nodes) == 1
    assert len(result.edges) == 1


# ---------------------------------------------------------------------------
# AnalysisInfo / Status / Id
# ---------------------------------------------------------------------------


def test_analysis_id_str():
    aid = AnalysisId(value="41f5bfb5-e1cd-4f2b-a56f-dc877a29365a")
    assert str(aid) == "41f5bfb5-e1cd-4f2b-a56f-dc877a29365a"


def test_analysis_status_str():
    status = AnalysisStatus(value="Started")
    assert str(status) == "Started"


def test_analysis_info_stores_all_fields():
    info = AnalysisInfo(
        id=AnalysisId(value="id-1"),
        project_id=ProjectId(value="proj-1"),
        instance_id=InstanceId(value="inst-1"),
        script_id=ScriptId(value="script-1"),
        status=AnalysisStatus(value="Running"),
        creation_date=datetime(2024, 1, 1, tzinfo=UTC),
    )
    assert info.id.value == "id-1"
    assert info.status.value == "Running"


# ---------------------------------------------------------------------------
# SampleCodeFile
# ---------------------------------------------------------------------------


def test_sample_code_file_stores_name_and_content():
    f = SampleCodeFile(name="file1.cs", content="class Foo {}")
    assert f.name == "file1.cs"
    assert f.content == "class Foo {}"


def test_sample_code_file_rejects_empty_name():
    with pytest.raises(Exception):
        SampleCodeFile(name="", content="x")


# ---------------------------------------------------------------------------
# PluginId
# ---------------------------------------------------------------------------


def test_plugin_id_str():
    pid = PluginId(name="my-plugin", version="1.2.3")
    assert str(pid) == "my-plugin@1.2.3"


# ---------------------------------------------------------------------------
# Plugin extension points
# ---------------------------------------------------------------------------


def test_nested_extension_point_kind():
    ep = NestedPluginExtensionPoint(entry_point="plugin.dll", version="1.0.0")
    assert ep.kind == PluginKind.PLUGIN


def test_loader_extension_point_kind():
    ep = LoaderPluginExtensionPoint(entry_point="loader.dll", version="2.0.0")
    assert ep.kind == PluginKind.LOADER


def test_linker_extension_point_kind():
    ep = LinkerPluginExtensionPoint(entry_point="linker.dll", version="1.0.0")
    assert ep.kind == PluginKind.LINKER


def test_script_generator_extension_point_extra_fields():
    ep = ScriptGeneratorPluginExtensionPoint(
        entry_point="gen.dll",
        version="1.0.0",
        language="csharp",
        extension=".cs",
    )
    assert ep.kind == PluginKind.SCRIPT_GENERATOR
    assert ep.language == "csharp"
    assert ep.extension == ".cs"


def test_script_runner_extension_point_extra_fields():
    ep = ScriptRunnerPluginExtensionPoint(
        entry_point="runner.dll",
        version="1.0.0",
        language="python",
        extension=".py",
    )
    assert ep.kind == PluginKind.SCRIPT_RUNNER
    assert ep.language == "python"


def test_helper_functions_extension_point_kind():
    ep = HelperFunctionsPluginExtensionPoint(entry_point="helpers.dll", version="1.0.0")
    assert ep.kind == PluginKind.HELPER_FUNCTIONS


def test_ui_extension_point_with_outlets():
    outlet = TopNavigationBarOutlet(
        exposed_module="./Module",
        label="Dashboard",
        path="/dashboard",
    )
    ep = UiPluginExtensionPoint(
        entry_point="ui.js",
        version="1.0.0",
        remote_name="myRemote",
        remote_entry="http://localhost/remoteEntry.js",
        outlets=[outlet],
    )
    assert ep.kind == PluginKind.UI
    assert len(ep.outlets) == 1
    assert ep.outlets[0].type == OutletType.TOP_NAVIGATION_BAR


def test_side_panel_outlet():
    o = SidePanelOutlet(
        exposed_module="./Panel",
        label="Panel",
        path="/panel",
        icon="icon-name",
    )
    assert o.type == OutletType.SIDE_PANEL
    assert o.icon == "icon-name"


def test_file_previewer_outlet():
    o = FilePreviewerOutlet(
        exposed_module="./Previewer",
        label="Previewer",
        supported_file_extensions=[".json", ".yaml"],
    )
    assert o.type == OutletType.FILE_PREVIEWER
    assert o.supported_file_extensions == [".json", ".yaml"]


# ---------------------------------------------------------------------------
# PluginManifest / Plugin
# ---------------------------------------------------------------------------


def test_plugin_manifest_stores_fields():
    manifest = PluginManifest(
        api_version="1.0",
        name="my-plugin",
        description="A plugin",
        author="dxworks",
        extension_points=[LoaderPluginExtensionPoint(entry_point="loader.dll", version="1.0.0")],
    )
    assert manifest.name == "my-plugin"
    assert len(manifest.extension_points) == 1


def test_plugin_manifest_optional_fields():
    manifest = PluginManifest(api_version="1.0", name="bare")
    assert manifest.description is None
    assert manifest.author is None
    assert manifest.extension_points == []


def test_plugin_stores_id_and_manifest():
    plugin = Plugin(
        folder_path="/plugins/my-plugin",
        id=PluginId(name="my-plugin", version="1.0.0"),
        manifest=PluginManifest(api_version="1.0", name="my-plugin"),
    )
    assert plugin.folder_path == "/plugins/my-plugin"
    assert plugin.id.name == "my-plugin"


# ---------------------------------------------------------------------------
# Script / ScriptLanguage
# ---------------------------------------------------------------------------


def test_script_stores_fields():
    script = Script(
        script_id=ScriptId(value="script-1"),
        project_id=ProjectId(value="proj-1"),
        path="src/analysis.cs",
        language=ScriptLanguage(name="csharp", extension=".cs"),
    )
    assert script.path == "src/analysis.cs"
    assert script.language.extension == ".cs"
    assert script.parameters == []
