using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Plugins.Marketplace.Errors;

public sealed record PluginVersionNotFoundError(PluginId Id);
