from dataclasses import dataclass

from scriptbee_analysis_sdk.domain.project import ProjectId, ScriptId


@dataclass(frozen=True)
class RunAnalysisCommand:
    project_id: ProjectId
    script_id: ScriptId
