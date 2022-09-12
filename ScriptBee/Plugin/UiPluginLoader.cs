using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public class UiPluginLoader : IPluginLoader
{
    private readonly IPluginService _pluginService;

    public UiPluginLoader(IPluginService pluginService)
    {
        _pluginService = pluginService;
    }

    public string AcceptedPluginKind => PluginTypes.Ui;

    public void Load(Models.Plugin plugin)
    {
        // todo see how to start the plugin via node or http-server or something like that if not already started
        _pluginService.Add(plugin.Manifest);
    }
}
