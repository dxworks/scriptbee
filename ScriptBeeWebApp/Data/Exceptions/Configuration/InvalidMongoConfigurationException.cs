using System;
using System.Runtime.Serialization;

namespace ScriptBeeWebApp.Exceptions.Configuration;

[Serializable]
public sealed class InvalidMongoConfigurationException : Exception
{
    public InvalidMongoConfigurationException(string message) : base(message)
    {
    }

    private InvalidMongoConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
