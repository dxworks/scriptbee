using System;
using System.Runtime.Serialization;

namespace ScriptBeeWebApp.Data.Exceptions;

[Serializable]
public sealed class ScriptGenerationStrategyNotFoundException : Exception
{
    public ScriptGenerationStrategyNotFoundException(string message) : base(message)
    {
    }

    private ScriptGenerationStrategyNotFoundException(SerializationInfo info, StreamingContext context) : base(info,
        context)
    {
    }
}
