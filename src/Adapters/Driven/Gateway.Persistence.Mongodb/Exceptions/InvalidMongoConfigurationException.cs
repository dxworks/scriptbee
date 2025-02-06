namespace ScriptBee.Gateway.Persistence.Mongodb.Exceptions;

[Serializable]
public sealed class InvalidMongoConfigurationException() : Exception("MongoDB connection string is not set");
