namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Exceptions;

[Serializable]
public class CompilationErrorException(string message) : Exception(message);
