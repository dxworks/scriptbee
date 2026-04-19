using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Marketplace.Client.Errors;

public sealed record PluginVersionNotFoundError(PluginId Id);
