namespace ScriptBee.Persistence.File.Plugin;

public interface IPluginDiscriminatorHolder
{
    Dictionary<string, Type> GetDiscriminatedTypes();
}
