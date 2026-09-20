from fastapi import APIRouter, Depends
from fastapi.responses import Response

from scriptbee_analysis_sdk._internal.endpoints.contracts.web_models import (
    WebRunAnalysisCommand,
    WebRunAnalysisResponse,
)
from scriptbee_analysis_sdk.abstractions.run_analysis_command import RunAnalysisCommand
from scriptbee_analysis_sdk.abstractions.run_analysis_use_case import RunAnalysisUseCase
from scriptbee_analysis_sdk.domain.project import ProjectId, ScriptId


def make_analyses_router(get_run_analysis: callable) -> APIRouter:
    router = APIRouter(tags=["Analysis"])

    @router.post(
        "/api/analyses",
        status_code=202,
        name="Analyses",
        summary="Run analysis",
        description="Starts the execution of an analysis script on the analysis service.",
    )
    async def run_analysis(
        command: WebRunAnalysisCommand,
        response: Response,
        use_case: RunAnalysisUseCase = Depends(get_run_analysis),
    ) -> WebRunAnalysisResponse:
        domain_command = RunAnalysisCommand(
            project_id=ProjectId(value=command.project_id),
            script_id=ScriptId(value=command.script_id),
        )
        info = await use_case.run(domain_command)
        response.headers["Location"] = f"/api/analyses/{info.id}"
        return WebRunAnalysisResponse.from_analysis_info(info)

    return router
