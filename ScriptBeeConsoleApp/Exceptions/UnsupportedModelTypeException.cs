using System;

namespace ScriptBeeConsoleApp.Exceptions
{
    public class UnsupportedModelTypeException : Exception
    {
        public UnsupportedModelTypeException(string message) : base(message)
        {
        }
    }
}