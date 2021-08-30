using System;

namespace ScriptRunnerConsoleApp.Exceptions
{
    public class UnsupportedScriptTypeException : Exception
    {
        public UnsupportedScriptTypeException(string message) : base(message)
        {
        }
    }
}