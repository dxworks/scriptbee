namespace ScriptBee.Marketplace.Client.Errors;

public sealed record PluginVersionNotFoundError(string PluginId, string Version);
