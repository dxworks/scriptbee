using System.Runtime.Serialization;

namespace ScriptBee.Marketplace.Client.Exceptions;

[Serializable]
public sealed class PluginVersionNotFoundException : Exception
{
    public PluginVersionNotFoundException(string message)
        : base(message) { }

    private PluginVersionNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
