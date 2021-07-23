using System;

namespace TemplateGeneratorConsoleApp.Exceptions
{
    public class UnsupportedScriptTypeException : Exception
    {
        public UnsupportedScriptTypeException(string message) : base(message)
        {
        }
    }
}