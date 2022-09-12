namespace ScriptBee.Plugin;

public interface IPluginLoader
{
    string AcceptedPluginKind { get; }

    // todo make this return Task
    void Load(Models.Plugin plugin);
}
