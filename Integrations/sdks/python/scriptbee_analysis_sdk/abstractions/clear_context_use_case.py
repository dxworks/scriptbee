from abc import ABC, abstractmethod


class ClearContextUseCase(ABC):
    @abstractmethod
    def clear(self) -> None: ...
