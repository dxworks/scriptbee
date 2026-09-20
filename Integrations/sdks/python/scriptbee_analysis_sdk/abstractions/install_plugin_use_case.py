from abc import ABC, abstractmethod
from dataclasses import dataclass

from scriptbee_analysis_sdk.abstractions.errors.invalid_plugin_error import InvalidPluginError
from scriptbee_analysis_sdk.abstractions.errors.plugin_installation_error import (
    PluginInstallationError,
)
from scriptbee_analysis_sdk.domain.plugins import PluginId


@dataclass(frozen=True)
class Success:
    pass


InstallPluginResult = Success | InvalidPluginError | PluginInstallationError


class InstallPluginUseCase(ABC):
    @abstractmethod
    def install_plugin(self, plugin_id: PluginId) -> InstallPluginResult: ...
