using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Plugins;

public sealed record UninstallPluginCommand(
    ProjectId ProjectId,
    string PluginId,
    string PluginVersion
);
