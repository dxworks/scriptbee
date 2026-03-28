using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Plugin;

public sealed record InstallPluginCommand(
    ProjectId ProjectId,
    string PluginId,
    string PluginVersion
);
