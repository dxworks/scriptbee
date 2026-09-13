"""Error types returned by use-case operations."""

from scriptbee_analysis_sdk.abstractions.errors.invalid_plugin_error import InvalidPluginError
from scriptbee_analysis_sdk.abstractions.errors.plugin_installation_error import (
    PluginInstallationError,
)

__all__ = ["InvalidPluginError", "PluginInstallationError"]
