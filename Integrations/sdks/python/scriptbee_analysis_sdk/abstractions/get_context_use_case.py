from abc import ABC, abstractmethod

from scriptbee_analysis_sdk.domain.context import ContextSlice


class GetContextUseCase(ABC):
    @abstractmethod
    def get(self) -> list[ContextSlice]: ...
