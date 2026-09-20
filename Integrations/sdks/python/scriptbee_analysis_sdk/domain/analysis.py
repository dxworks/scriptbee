from datetime import datetime
from enum import StrEnum

from pydantic import BaseModel

from scriptbee_analysis_sdk.domain.project import InstanceId, ProjectId, ScriptId


class AnalysisId(BaseModel):
    value: str

    def __str__(self) -> str:
        return self.value


class AnalysisStatus(BaseModel):
    value: str

    def __str__(self) -> str:
        return self.value


class ResultId(BaseModel):
    value: str

    def __str__(self) -> str:
        return self.value


class ResultType(StrEnum):
    FILE = "File"
    CONSOLE = "Console"
    RUN_ERROR = "RunError"


class ResultSummary(BaseModel):
    id: ResultId
    name: str
    type: str
    creation_date: datetime


class AnalysisInfo(BaseModel):
    id: AnalysisId
    project_id: ProjectId
    instance_id: InstanceId
    script_id: ScriptId
    status: AnalysisStatus
    creation_date: datetime
    results: list[ResultSummary] = []
    errors: list[str] = []
    finished_date: datetime | None = None

    def is_running(self) -> bool:
        return self.status.value in ("Started", "Running")
