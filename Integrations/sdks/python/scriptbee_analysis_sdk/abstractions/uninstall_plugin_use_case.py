from abc import ABC, abstractmethod

from scriptbee_analysis_sdk.domain.plugins import PluginId


class UninstallPluginUseCase(ABC):
    @abstractmethod
    def uninstall_plugin(self, plugin_id: PluginId) -> None: ...
