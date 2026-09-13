from abc import ABC, abstractmethod

from scriptbee_analysis_sdk.domain.context import ContextGraphResult


class GetContextGraphUseCase(ABC):
    @abstractmethod
    def search_nodes(self, query: str, offset: int, limit: int) -> ContextGraphResult: ...

    @abstractmethod
    def get_neighbors(self, node_id: str) -> ContextGraphResult: ...
