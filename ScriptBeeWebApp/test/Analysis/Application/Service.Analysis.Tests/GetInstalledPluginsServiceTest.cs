using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Plugins.Loader;
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
            new("folder", new PluginId("id", new Version(1, 2, 3)), new PluginManifest()),
        ];
        _pluginRepository.GetLoadedPlugins().Returns(expectedPlugins);

        var plugins = await _getInstalledPluginsService.Get(TestContext.Current.CancellationToken);

        plugins.ToList().ShouldBeEquivalentTo(expectedPlugins);
    }
}
