using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Domain.Model.Errors;

public sealed record PluginInstallationError(
    PluginId Id,
    List<PluginId> NestedPluginsThatCouldNotBeInstalled
);
