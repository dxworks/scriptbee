namespace ScriptBee.Marketplace.Client.Exceptions;

[Serializable]
public sealed class PluginVersionNotFoundException(string message) : Exception(message);
