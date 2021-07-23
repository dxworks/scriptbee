using System;

namespace ScriptBeeConsoleApp.Exceptions
{
    public class UnsupportedScriptTypeException : Exception
    {
        public UnsupportedScriptTypeException(string message) : base(message)
        {
        }
    }
}