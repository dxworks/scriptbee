namespace ScriptBee.Domain.Model.Errors;

public sealed record PluginInstallationError(string Name, string Version);
