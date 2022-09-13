namespace ScriptBee.Plugin;

public interface IPluginLoader
{
    // todo make this return Task
    void Load(Models.Plugin plugin);
}
