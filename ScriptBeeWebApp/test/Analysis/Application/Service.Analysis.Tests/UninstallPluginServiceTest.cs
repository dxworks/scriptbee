using NSubstitute;
using ScriptBee.Plugins.Loader;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class UninstallPluginServiceTest
{
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();

    private readonly UninstallPluginService _uninstallPluginService;

    public UninstallPluginServiceTest()
    {
        _uninstallPluginService = new UninstallPluginService(_pluginRepository);
    }

    [Fact]
    public void UninstallPlugin_UnregistersEachUninstalledPluginVersion()
    {
        const string pluginId = "testPlugin";
        const string pluginVersion = "1.0.0";

        _uninstallPluginService.UninstallPlugin(pluginId, pluginVersion);

        _pluginRepository.Received(1).UnRegisterPlugin("testPlugin", "1.0.0");
    }
}
