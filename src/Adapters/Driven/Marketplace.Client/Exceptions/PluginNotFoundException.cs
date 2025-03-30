using System.Runtime.Serialization;

namespace ScriptBee.Marketplace.Client.Exceptions;

[Serializable]
public sealed class PluginNotFoundException : Exception
{
    public PluginNotFoundException(string message)
        : base(message) { }

    private PluginNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
