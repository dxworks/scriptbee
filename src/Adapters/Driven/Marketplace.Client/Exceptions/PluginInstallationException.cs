using System.Runtime.Serialization;

namespace ScriptBee.Marketplace.Client.Exceptions;

[Serializable]
public class PluginInstallationException : Exception
{
    public PluginInstallationException(string name, string version)
        : base($"Plugin with name '{name}' and version '{version}' could not be installed.") { }

    protected PluginInstallationException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
