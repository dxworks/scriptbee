namespace ScriptBee.Plugin;

public interface IPluginRepository
{
    void RegisterPlugin<T>(object argument);

    T? GetPlugin<T>(object argument);
}
