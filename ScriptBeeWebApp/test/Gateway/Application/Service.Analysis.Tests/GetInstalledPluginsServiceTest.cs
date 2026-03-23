using NSubstitute;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class GetInstalledPluginsServiceTest
{
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();

    private readonly GetInstalledPluginsService _getInstalledPluginsService;

    public GetInstalledPluginsServiceTest()
    {
        _getInstalledPluginsService = new GetInstalledPluginsService(_pluginRepository);
    }

    [Fact]
    public async Task GetInstalledPluginsService()
    {
        List<Plugin> expectedPlugins =
        [
            new("folder", "id", new Version(1, 2, 3), new PluginManifest()),
        ];
        _pluginRepository.GetLoadedPlugins().Returns(expectedPlugins);

        var plugins = await _getInstalledPluginsService.Get(TestContext.Current.CancellationToken);

        plugins.ToList().ShouldBeEquivalentTo(expectedPlugins);
    }
}
