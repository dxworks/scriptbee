using NSubstitute;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Plugins.Installer;

namespace ScriptBee.Service.Plugin.Tests;

public class UninstallPluginServiceTest
{
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();

    private readonly IBundlePluginUninstaller _bundlePluginUninstaller =
        Substitute.For<IBundlePluginUninstaller>();

    private readonly UninstallPluginService _uninstallPluginService;

    public UninstallPluginServiceTest()
    {
        _uninstallPluginService = new UninstallPluginService(
            _pluginRepository,
            _bundlePluginUninstaller
        );
    }

    [Fact]
    public void UninstallPlugin_UnregistersEachUninstalledPluginVersion()
    {
        const string pluginId = "testPlugin";
        const string pluginVersion = "1.0.0";
        var uninstalledVersions = new List<(string pluginId, string version)>
        {
            ("testPlugin", "1.0.0"),
            ("anotherPlugin", "2.0.0"),
        };
        _bundlePluginUninstaller.Uninstall(pluginId, pluginVersion).Returns(uninstalledVersions);

        _uninstallPluginService.UninstallPlugin(pluginId, pluginVersion);

        _pluginRepository.Received(1).UnRegisterPlugin("testPlugin", "1.0.0");
        _pluginRepository.Received(1).UnRegisterPlugin("anotherPlugin", "2.0.0");
    }

    [Fact]
    public void UninstallPlugin_HandlesEmptyUninstalledVersions()
    {
        const string pluginId = "testPlugin";
        const string pluginVersion = "1.0.0";
        _bundlePluginUninstaller
            .Uninstall(pluginId, pluginVersion)
            .Returns(new List<(string pluginId, string version)>());

        _uninstallPluginService.UninstallPlugin(pluginId, pluginVersion);

        _pluginRepository.DidNotReceive().UnRegisterPlugin(Arg.Any<string>(), Arg.Any<string>());
    }
}
