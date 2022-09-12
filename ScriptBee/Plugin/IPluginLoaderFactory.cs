namespace ScriptBee.Plugin;

public interface IPluginLoaderFactory
{
    IPluginLoader? GetPluginLoader(Models.Plugin plugin);
}
