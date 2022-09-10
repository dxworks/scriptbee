namespace ScriptBee.Plugin;

public interface IPluginRepository
{
    void RegisterPlugin<T>(string argument);

    T? GetPlugin<T>(string argument);
}
