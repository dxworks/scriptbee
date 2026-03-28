using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Plugin;

public sealed record UninstallPluginCommand(
    ProjectId ProjectId,
    string PluginId,
    string PluginVersion
);
