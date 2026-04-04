namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Exceptions;

[Serializable]
public class InvalidParameterTypeException(string parameterType)
    : Exception($"Invalid parameter type: {parameterType}");
