from datetime import datetime

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


class AnalysisInfo(BaseModel):
    id: AnalysisId
    project_id: ProjectId
    instance_id: InstanceId
    script_id: ScriptId
    status: AnalysisStatus
    creation_date: datetime
