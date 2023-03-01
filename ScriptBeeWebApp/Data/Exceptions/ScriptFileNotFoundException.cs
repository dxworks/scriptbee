using System.Runtime.Serialization;

namespace ScriptBeeWebApp.Data.Exceptions;

[Serializable]
public sealed class ScriptFileNotFoundException : Exception
{
    public ScriptFileNotFoundException(string message) : base(message)
    {
    }

    private ScriptFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
