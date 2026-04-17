namespace ScriptBee.Common.Plugins;

public interface IPluginDiscriminatorHolder
{
    Dictionary<string, Type> GetDiscriminatedTypes();
}
