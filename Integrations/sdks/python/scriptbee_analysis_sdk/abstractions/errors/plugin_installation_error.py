from dataclasses import dataclass

from scriptbee_analysis_sdk.domain.plugins import PluginId


@dataclass(frozen=True)
class PluginInstallationError:
    id: PluginId
