using System;

namespace TemplateGeneratorConsoleApp.Exceptions
{
    public class UnsupportedModelTypeException : Exception
    {
        public UnsupportedModelTypeException(string message) : base(message)
        {
        }
    }
}