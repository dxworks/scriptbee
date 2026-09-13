from httpx import ASGITransport, AsyncClient

from scriptbee_analysis_sdk.abstractions import RunAnalysisCommand, RunAnalysisUseCase
from tests.endpoints.conftest import StubContainer, make_analysis_info, make_app


class RunAnalysisStub(RunAnalysisUseCase):
    def __init__(self, result):
        self._result = result
        self.received_command = None

    async def run(self, command: RunAnalysisCommand):
        self.received_command = command
        return self._result


async def test_run_analysis_returns_202_with_location():
    stub = RunAnalysisStub(make_analysis_info())
    app = make_app(StubContainer(run_analysis=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post(
            "/api/analyses", json={"project_id": "proj-1", "script_id": "script-1"}
        )

    assert response.status_code == 202
    assert response.headers["location"] == "/api/analyses/analysis-id-1"
    body = response.json()
    assert body["id"] == "analysis-id-1"
    assert body["project_id"] == "proj-1"
    assert body["script_id"] == "script-1"
    assert body["status"] == "Started"


async def test_run_analysis_passes_correct_command():
    stub = RunAnalysisStub(make_analysis_info())
    app = make_app(StubContainer(run_analysis=stub))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        await c.post("/api/analyses", json={"project_id": "my-project", "script_id": "my-script"})

    assert stub.received_command.project_id.value == "my-project"
    assert stub.received_command.script_id.value == "my-script"


async def test_run_analysis_returns_422_when_project_id_empty():
    app = make_app(StubContainer(run_analysis=RunAnalysisStub(None)))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/analyses", json={"project_id": "", "script_id": "s"})

    assert response.status_code == 422


async def test_run_analysis_returns_422_when_script_id_empty():
    app = make_app(StubContainer(run_analysis=RunAnalysisStub(None)))

    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as c:
        response = await c.post("/api/analyses", json={"project_id": "p", "script_id": ""})

    assert response.status_code == 422
