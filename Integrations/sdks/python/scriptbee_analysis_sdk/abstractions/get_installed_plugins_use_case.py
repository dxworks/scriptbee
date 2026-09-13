from abc import ABC, abstractmethod

from scriptbee_analysis_sdk.domain.plugins import Plugin


class GetInstalledPluginsUseCase(ABC):
    @abstractmethod
    def get(self) -> list[Plugin]: ...
