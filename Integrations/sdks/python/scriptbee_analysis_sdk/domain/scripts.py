from pydantic import BaseModel

from scriptbee_analysis_sdk.domain.project import ProjectId, ScriptId


class ScriptLanguage(BaseModel):
    name: str
    extension: str


class Script(BaseModel):
    script_id: ScriptId
    project_id: ProjectId
    path: str
    language: ScriptLanguage
    parameters: list[str] = []
