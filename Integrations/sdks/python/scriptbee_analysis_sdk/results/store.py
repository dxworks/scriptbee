from typing import Protocol


class ScriptResultsStore(Protocol):
    async def upload_file(
        self,
        file_id: str,
        content: bytes,
        metadata: dict[str, str] | None = None,
    ) -> None: ...
