namespace ScriptBee.Persistence.File.Exceptions;

[Serializable]
public class PluginVersionExistsException(string name, string version)
    : Exception($"Plugin with name '{name}' and version '{version}' already exists");
