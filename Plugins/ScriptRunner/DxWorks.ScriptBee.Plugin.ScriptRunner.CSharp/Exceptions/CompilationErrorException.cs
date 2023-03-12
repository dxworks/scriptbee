using System.Runtime.Serialization;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Exceptions;

[Serializable]
public class CompilationErrorException : Exception
    {
        public CompilationErrorException(string message) : base(message)
        {
        }

        protected CompilationErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
}
