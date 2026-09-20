from typing import Protocol


class ScriptResultsStore(Protocol):
    async def upload_file(
        self,
        file_id: str,
        content: bytes,
        metadata: dict[str, str] | None = None,
    ) -> None: ...


class InMemoryScriptResultsStore:
    def __init__(self) -> None:
        self._files: dict[str, bytes] = {}
        self._metadata: dict[str, dict[str, str] | None] = {}

    async def upload_file(
        self,
        file_id: str,
        content: bytes,
        metadata: dict[str, str] | None = None,
    ) -> None:
        self._files[file_id] = content
        self._metadata[file_id] = metadata

    def get_file(self, file_id: str) -> bytes | None:
        return self._files.get(file_id)

    def has_file(self, file_id: str) -> bool:
        return file_id in self._files
