using System;
using System.Runtime.Serialization;

namespace ScriptBeeWebApp.Data.Exceptions;

[Serializable]
public sealed class ScriptRunnerNotFoundException : Exception
{
    public ScriptRunnerNotFoundException(string message) : base(message)
    {
    }

    private ScriptRunnerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
