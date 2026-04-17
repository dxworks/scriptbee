namespace ScriptBee.Plugins;

public interface IPluginDiscriminatorHolder
{
    Dictionary<string, Type> GetDiscriminatedTypes();
}
