from abc import ABC, abstractmethod


class LinkContextUseCase(ABC):
    @abstractmethod
    async def link(self, linker_ids: list[str]) -> None: ...
