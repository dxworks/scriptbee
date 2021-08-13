using System;

namespace ScriptSampleGeneratorConsoleApp.Exceptions
{
    public class UnsupportedScriptTypeException : Exception
    {
        public UnsupportedScriptTypeException(string message) : base(message)
        {
        }
    }
}