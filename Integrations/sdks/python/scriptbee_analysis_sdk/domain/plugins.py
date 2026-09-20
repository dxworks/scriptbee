from __future__ import annotations

from enum import StrEnum
from typing import Annotated, Literal

from pydantic import BaseModel, Field


class PluginKind(StrEnum):
    PLUGIN = "Plugin"
    LOADER = "Loader"
    LINKER = "Linker"
    SCRIPT_GENERATOR = "ScriptGenerator"
    SCRIPT_RUNNER = "ScriptRunner"
    HELPER_FUNCTIONS = "HelperFunctions"
    UI = "Ui"


class OutletType(StrEnum):
    TOP_NAVIGATION_BAR = "TopNavigationBar"
    SIDE_PANEL = "SidePanel"
    FILE_PREVIEWER = "FilePreviewer"


class PluginId(BaseModel):
    name: str
    version: str

    def __str__(self) -> str:
        return f"{self.name}@{self.version}"


class TopNavigationBarOutlet(BaseModel):
    type: Literal[OutletType.TOP_NAVIGATION_BAR] = OutletType.TOP_NAVIGATION_BAR
    exposed_module: str
    label: str
    path: str
    nested: bool | None = None
    component_name: str | None = None


class SidePanelOutlet(BaseModel):
    type: Literal[OutletType.SIDE_PANEL] = OutletType.SIDE_PANEL
    exposed_module: str
    label: str
    path: str
    nested: bool | None = None
    component_name: str | None = None
    icon: str = ""


class FilePreviewerOutlet(BaseModel):
    type: Literal[OutletType.FILE_PREVIEWER] = OutletType.FILE_PREVIEWER
    exposed_module: str
    label: str
    component_name: str | None = None
    icon: str | None = None
    supported_file_extensions: list[str] | None = None


UiPluginExtensionPointOutlet = Annotated[
    TopNavigationBarOutlet | SidePanelOutlet | FilePreviewerOutlet,
    Field(discriminator="type"),
]


class NestedPluginExtensionPoint(BaseModel):
    kind: Literal[PluginKind.PLUGIN] = PluginKind.PLUGIN
    entry_point: str
    version: str


class LoaderPluginExtensionPoint(BaseModel):
    kind: Literal[PluginKind.LOADER] = PluginKind.LOADER
    entry_point: str
    version: str


class LinkerPluginExtensionPoint(BaseModel):
    kind: Literal[PluginKind.LINKER] = PluginKind.LINKER
    entry_point: str
    version: str


class ScriptGeneratorPluginExtensionPoint(BaseModel):
    kind: Literal[PluginKind.SCRIPT_GENERATOR] = PluginKind.SCRIPT_GENERATOR
    entry_point: str
    version: str
    language: str
    extension: str


class ScriptRunnerPluginExtensionPoint(BaseModel):
    kind: Literal[PluginKind.SCRIPT_RUNNER] = PluginKind.SCRIPT_RUNNER
    entry_point: str
    version: str
    language: str
    extension: str


class HelperFunctionsPluginExtensionPoint(BaseModel):
    kind: Literal[PluginKind.HELPER_FUNCTIONS] = PluginKind.HELPER_FUNCTIONS
    entry_point: str
    version: str


class UiPluginExtensionPoint(BaseModel):
    kind: Literal[PluginKind.UI] = PluginKind.UI
    entry_point: str
    version: str
    remote_name: str
    remote_entry: str
    outlets: list[UiPluginExtensionPointOutlet] = []


PluginExtensionPoint = Annotated[
    NestedPluginExtensionPoint
    | LoaderPluginExtensionPoint
    | LinkerPluginExtensionPoint
    | ScriptGeneratorPluginExtensionPoint
    | ScriptRunnerPluginExtensionPoint
    | HelperFunctionsPluginExtensionPoint
    | UiPluginExtensionPoint,
    Field(discriminator="kind"),
]


class PluginManifest(BaseModel):
    api_version: str
    name: str
    description: str | None = None
    author: str | None = None
    extension_points: list[PluginExtensionPoint] = []


class Plugin(BaseModel):
    folder_path: str
    id: PluginId
    manifest: PluginManifest
