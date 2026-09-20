from collections.abc import Callable
from datetime import UTC, datetime
from typing import Protocol
from uuid import uuid4

from scriptbee_analysis_sdk.domain.analysis import ResultId, ResultSummary, ResultType
from scriptbee_analysis_sdk.results.store import ScriptResultsStore


class AnalysisResultService(Protocol):
    async def add_file(
        self,
        name: str,
        content: str | bytes,
        result_type: str = ResultType.FILE,
    ) -> ResultId: ...

    async def add_console(
        self,
        content: str,
        name: str = "ConsoleOutput",
    ) -> ResultId: ...

    async def add_error(
        self,
        message: str,
        name: str = "RunError",
    ) -> ResultId: ...

    async def add_result(
        self,
        name: str,
        result_type: str,
        content: str | bytes,
    ) -> ResultId: ...


class DefaultAnalysisResultService:
    def __init__(
        self,
        store: ScriptResultsStore,
        id_generator: Callable[[], str] | None = None,
        date_provider: Callable[[], datetime] | None = None,
        on_result_added: Callable[[ResultSummary], None] | None = None,
    ) -> None:
        self._store = store
        self._id_generator = id_generator or (lambda: str(uuid4()))
        self._date_provider = date_provider or (lambda: datetime.now(UTC))
        self._on_result_added = on_result_added

    async def add_file(
        self,
        name: str,
        content: str | bytes,
        result_type: str = ResultType.FILE,
    ) -> ResultId:
        return await self.add_result(name=name, result_type=result_type, content=content)

    async def add_console(
        self,
        content: str,
        name: str = "ConsoleOutput",
    ) -> ResultId:
        return await self.add_result(name=name, result_type=ResultType.CONSOLE, content=content)

    async def add_error(
        self,
        message: str,
        name: str = "RunError",
    ) -> ResultId:
        return await self.add_result(name=name, result_type=ResultType.RUN_ERROR, content=message)

    async def add_result(
        self,
        name: str,
        result_type: str,
        content: str | bytes,
    ) -> ResultId:
        raw_bytes = content.encode("utf-8") if isinstance(content, str) else content
        result_id = ResultId(value=self._id_generator())
        await self._store.upload_file(file_id=result_id.value, content=raw_bytes)

        if self._on_result_added is not None:
            summary = ResultSummary(
                id=result_id,
                name=name,
                type=result_type,
                creation_date=self._date_provider(),
            )
            self._on_result_added(summary)

        return result_id
