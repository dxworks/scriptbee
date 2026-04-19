using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Domain.Model.Errors;

public sealed record PluginNotFoundError(PluginId Id);
