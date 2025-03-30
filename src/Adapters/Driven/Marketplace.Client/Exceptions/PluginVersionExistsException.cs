using System.Runtime.Serialization;

namespace ScriptBee.Marketplace.Client.Exceptions;

[Serializable]
public class PluginVersionExistsException : Exception
{
    public PluginVersionExistsException(string name, string version)
        : base($"Plugin with name '{name}' and version '{version}' already exists") { }

    protected PluginVersionExistsException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
