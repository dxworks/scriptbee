using System;
using System.Runtime.Serialization;

namespace ScriptBeeWebApp.Data.Exceptions;

[Serializable]
public sealed class ProjectFolderNotFoundException : Exception
{
    public ProjectFolderNotFoundException(string message) : base(message)
    {
    }
    
    public ProjectFolderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
