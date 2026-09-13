from pydantic import BaseModel


class ContextSlice(BaseModel):
    model: str
    plugin_ids: list[str]


class ContextGraphNode(BaseModel):
    id: str
    label: str
    type: str
    loader: str | None = None
    properties: dict[str, object] = {}


class ContextGraphEdge(BaseModel):
    source: str
    target: str
    label: str


class ContextGraphResult(BaseModel):
    nodes: list[ContextGraphNode]
    edges: list[ContextGraphEdge]
