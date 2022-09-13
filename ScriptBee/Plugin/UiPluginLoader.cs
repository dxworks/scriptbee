using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public class UiPluginLoader : IPluginLoader
{
    private readonly IPluginRepository _pluginRepository;

    public UiPluginLoader(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    public string AcceptedPluginKind => PluginTypes.Ui;

    public void Load(Models.Plugin plugin)
    {
        // todo see how to start the plugin via node or http-server or something like that if not already started
        _pluginRepository.RegisterPlugin(plugin.Manifest);
    }
}
