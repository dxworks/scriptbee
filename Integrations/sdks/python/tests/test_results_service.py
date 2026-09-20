from datetime import UTC, datetime

import pytest

from scriptbee_analysis_sdk.domain.analysis import ResultSummary, ResultType
from scriptbee_analysis_sdk.results.service import DefaultAnalysisResultService
from scriptbee_analysis_sdk.results.store import InMemoryScriptResultsStore


@pytest.mark.asyncio
async def test_add_file_with_string_content() -> None:
    store = InMemoryScriptResultsStore()
    fixed_date = datetime(2026, 1, 1, 12, 0, 0, tzinfo=UTC)
    recorded_summaries: list[ResultSummary] = []

    service = DefaultAnalysisResultService(
        store=store,
        id_generator=lambda: "custom-id-1",
        date_provider=lambda: fixed_date,
        on_result_added=recorded_summaries.append,
    )

    result_id = await service.add_file("output.txt", "file content")

    assert result_id.value == "custom-id-1"
    assert store.get_file("custom-id-1") == b"file content"
    assert len(recorded_summaries) == 1
    assert recorded_summaries[0].id.value == "custom-id-1"
    assert recorded_summaries[0].name == "output.txt"
    assert recorded_summaries[0].type == ResultType.FILE
    assert recorded_summaries[0].creation_date == fixed_date


@pytest.mark.asyncio
async def test_add_file_with_bytes_content() -> None:
    store = InMemoryScriptResultsStore()
    service = DefaultAnalysisResultService(
        store=store,
        id_generator=lambda: "custom-id-bytes",
    )

    result_id = await service.add_file("binary.bin", b"\x00\x01\x02")

    assert result_id.value == "custom-id-bytes"
    assert store.get_file("custom-id-bytes") == b"\x00\x01\x02"


@pytest.mark.asyncio
async def test_add_console() -> None:
    store = InMemoryScriptResultsStore()
    recorded_summaries: list[ResultSummary] = []
    service = DefaultAnalysisResultService(
        store=store,
        id_generator=lambda: "console-id",
        on_result_added=recorded_summaries.append,
    )

    result_id = await service.add_console("log line")

    assert result_id.value == "console-id"
    assert store.get_file("console-id") == b"log line"
    assert len(recorded_summaries) == 1
    assert recorded_summaries[0].name == "ConsoleOutput"
    assert recorded_summaries[0].type == ResultType.CONSOLE


@pytest.mark.asyncio
async def test_add_error() -> None:
    store = InMemoryScriptResultsStore()
    recorded_summaries: list[ResultSummary] = []
    service = DefaultAnalysisResultService(
        store=store,
        id_generator=lambda: "error-id",
        on_result_added=recorded_summaries.append,
    )

    result_id = await service.add_error("Something went wrong")

    assert result_id.value == "error-id"
    assert store.get_file("error-id") == b"Something went wrong"
    assert len(recorded_summaries) == 1
    assert recorded_summaries[0].name == "RunError"
    assert recorded_summaries[0].type == ResultType.RUN_ERROR


@pytest.mark.asyncio
async def test_add_custom_result() -> None:
    store = InMemoryScriptResultsStore()
    recorded_summaries: list[ResultSummary] = []
    service = DefaultAnalysisResultService(
        store=store,
        id_generator=lambda: "custom-type-id",
        on_result_added=recorded_summaries.append,
    )

    result_id = await service.add_result(
        name="CustomChart",
        result_type="Chart",
        content="chart-data",
    )

    assert result_id.value == "custom-type-id"
    assert store.get_file("custom-type-id") == b"chart-data"
    assert len(recorded_summaries) == 1
    assert recorded_summaries[0].name == "CustomChart"
    assert recorded_summaries[0].type == "Chart"


@pytest.mark.asyncio
async def test_default_providers() -> None:
    store = InMemoryScriptResultsStore()
    service = DefaultAnalysisResultService(store=store)

    result_id = await service.add_file("default.txt", "test")

    assert result_id.value != ""
    assert store.has_file(result_id.value)
