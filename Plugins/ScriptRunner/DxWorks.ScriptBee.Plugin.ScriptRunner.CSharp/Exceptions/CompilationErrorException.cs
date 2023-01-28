using System;

namespace ScriptBee.Scripts.ScriptRunners.Exceptions
{
    public class CompilationErrorException : Exception
    {
        public CompilationErrorException(string message) : base(message)
        {
        }
    }
}