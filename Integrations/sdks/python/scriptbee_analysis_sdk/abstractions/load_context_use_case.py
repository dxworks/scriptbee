from abc import ABC, abstractmethod

from scriptbee_analysis_sdk.domain.project import FileId


class LoadContextUseCase(ABC):
    @abstractmethod
    async def load(self, files_to_load: dict[str, list[FileId]]) -> None: ...
