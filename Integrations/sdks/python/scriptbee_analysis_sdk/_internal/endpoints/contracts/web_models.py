from datetime import datetime

from pydantic import BaseModel, field_validator

from scriptbee_analysis_sdk.domain.analysis import AnalysisInfo
from scriptbee_analysis_sdk.domain.context import (
    ContextGraphEdge,
    ContextGraphNode,
    ContextSlice,
)
from scriptbee_analysis_sdk.domain.plugins import Plugin, PluginManifest


class WebRunAnalysisCommand(BaseModel):
    project_id: str
    script_id: str

    @field_validator("project_id", "script_id")
    @classmethod
    def _not_empty(cls, v: str) -> str:
        if not v or not v.strip():
            raise ValueError("must not be empty")
        return v


class WebRunAnalysisResponse(BaseModel):
    id: str
    project_id: str
    script_id: str
    status: str
    creation_date: datetime

    @classmethod
    def from_analysis_info(cls, info: AnalysisInfo) -> "WebRunAnalysisResponse":
        return cls(
            id=str(info.id),
            project_id=str(info.project_id),
            script_id=str(info.script_id),
            status=str(info.status),
            creation_date=info.creation_date,
        )


class WebLoadContextCommand(BaseModel):
    files_to_load: dict[str, list[str]]

    @field_validator("files_to_load")
    @classmethod
    def _not_null(cls, v):
        if v is None:
            raise ValueError("must not be null")
        return v


class WebLinkContextCommand(BaseModel):
    linker_ids: list[str]

    @field_validator("linker_ids")
    @classmethod
    def _not_null(cls, v):
        if v is None:
            raise ValueError("must not be null")
        return v


class WebContextSlice(BaseModel):
    model: str
    plugin_ids: list[str]

    @classmethod
    def from_context_slice(cls, cs: ContextSlice) -> "WebContextSlice":
        return cls(model=cs.model, plugin_ids=cs.plugin_ids)


class WebGetContextResponse(BaseModel):
    data: list[WebContextSlice]


class WebGenerateClassesRequest(BaseModel):
    languages: list[str] | None = None
    transfer_format: str | None = None


class WebContextGraphNode(BaseModel):
    id: str
    label: str
    type: str
    loader: str | None = None
    properties: dict[str, object] = {}

    @classmethod
    def from_node(cls, node: ContextGraphNode) -> "WebContextGraphNode":
        return cls(
            id=node.id,
            label=node.label,
            type=node.type,
            loader=node.loader,
            properties=node.properties,
        )


class WebContextGraphEdge(BaseModel):
    source: str
    target: str
    label: str

    @classmethod
    def from_edge(cls, edge: ContextGraphEdge) -> "WebContextGraphEdge":
        return cls(source=edge.source, target=edge.target, label=edge.label)


class WebContextGraphResponse(BaseModel):
    nodes: list[WebContextGraphNode]
    edges: list[WebContextGraphEdge]


class WebInstallPluginCommand(BaseModel):
    plugin_id: str
    version: str

    @field_validator("plugin_id", "version")
    @classmethod
    def _not_empty(cls, v: str) -> str:
        if not v or not v.strip():
            raise ValueError("must not be empty")
        return v


class WebInstalledPluginManifest(BaseModel):
    api_version: str
    name: str
    description: str | None = None
    author: str | None = None
    extension_points: list = []

    @classmethod
    def from_manifest(cls, manifest: PluginManifest) -> "WebInstalledPluginManifest":
        return cls(
            api_version=manifest.api_version,
            name=manifest.name,
            description=manifest.description,
            author=manifest.author,
            extension_points=manifest.extension_points,
        )


class WebInstalledPlugin(BaseModel):
    folder_path: str
    id: str
    version: str
    manifest: WebInstalledPluginManifest

    @classmethod
    def from_plugin(cls, plugin: Plugin) -> "WebInstalledPlugin":
        return cls(
            folder_path=plugin.folder_path,
            id=plugin.id.name,
            version=plugin.id.version,
            manifest=WebInstalledPluginManifest.from_manifest(plugin.manifest),
        )


class WebGetInstalledPluginsResponse(BaseModel):
    data: list[WebInstalledPlugin]
