using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Plugins;

public sealed record UninstallPluginCommand(
    ProjectId ProjectId,
    string PluginId,
    string PluginVersion
);
