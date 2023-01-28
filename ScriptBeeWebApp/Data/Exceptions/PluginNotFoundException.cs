using System;
using System.Runtime.Serialization;

namespace ScriptBeeWebApp.Data.Exceptions;

[Serializable]
public sealed class PluginNotFoundException : Exception
{
    public PluginNotFoundException(string message) : base(message)
    {
    }

    private PluginNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
