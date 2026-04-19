using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Plugins;

public sealed record InstallPluginCommand(ProjectId ProjectId, PluginId PluginId);
