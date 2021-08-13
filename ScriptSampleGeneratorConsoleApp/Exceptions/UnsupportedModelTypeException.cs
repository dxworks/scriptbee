using System;

namespace ScriptSampleGeneratorConsoleApp.Exceptions
{
    public class UnsupportedModelTypeException : Exception
    {
        public UnsupportedModelTypeException(string message) : base(message)
        {
        }
    }
}