namespace ScriptBee.Persistence.File;

public interface IPluginDiscriminatorHolder
{
    Dictionary<string, Type> GetDiscriminatedTypes();
}
