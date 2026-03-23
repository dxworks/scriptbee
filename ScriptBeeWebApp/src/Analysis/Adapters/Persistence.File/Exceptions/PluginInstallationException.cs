namespace ScriptBee.Persistence.File.Exceptions;

[Serializable]
public class PluginInstallationException(string name, string version)
    : Exception($"Plugin with name '{name}' and version '{version}' could not be installed.");
