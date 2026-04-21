using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class UninstallPluginServiceTest
{
    private readonly IPluginLoader _pluginLoader = Substitute.For<IPluginLoader>();

    private readonly UninstallPluginService _uninstallPluginService;

    public UninstallPluginServiceTest()
    {
        _uninstallPluginService = new UninstallPluginService(_pluginLoader);
    }

    [Fact]
    public void UninstallPlugin_UnregistersEachUninstalledPluginVersion()
    {
        var pluginId = new PluginId("testPlugin", new Version("1.0.0"));

        _uninstallPluginService.UninstallPlugin(pluginId);

        _pluginLoader.Received(1).Unload(pluginId);
    }
}
