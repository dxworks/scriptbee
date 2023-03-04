using System.Runtime.Serialization;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Exceptions;

[Serializable]
public class InvalidParameterTypeException : Exception
{
    public InvalidParameterTypeException(string parameterType) : base($"Invalid parameter type: {parameterType}")
    {
    }

    protected InvalidParameterTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
