using System;
using System.Runtime.Serialization;

namespace ScriptBeeWebApp.Data.Exceptions;

[Serializable]
public sealed class PluginVersionNotFoundException : Exception
{
    public PluginVersionNotFoundException(string message) : base(message)
    {
    }

    private PluginVersionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
