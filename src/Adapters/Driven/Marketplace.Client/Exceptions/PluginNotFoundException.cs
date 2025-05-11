namespace ScriptBee.Marketplace.Client.Exceptions;

[Serializable]
public sealed class PluginNotFoundException(string message) : Exception(message);
