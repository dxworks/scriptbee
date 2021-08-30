using System;

namespace ScriptRunnerConsoleApp.Exceptions
{
    public class UnsupportedModelTypeException : Exception
    {
        public UnsupportedModelTypeException(string message) : base(message)
        {
        }
    }
}