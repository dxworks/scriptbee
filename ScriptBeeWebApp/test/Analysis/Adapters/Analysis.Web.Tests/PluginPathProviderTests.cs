using Microsoft.Extensions.Options;
using ScriptBee.Service.Plugin;
using ScriptBee.Service.Plugin.Config;

namespace ScriptBee.Analysis.Web.Tests;

public class PluginPathProviderTests
{
    [Fact]
    public void GivenInstallationFolder_WhenGetPluginPath_ThenReturnPluginPath()
    {
        var options = Options.Create(new PluginsSettings { InstallationFolder = "folder" });

        var pluginPathProvider = new PluginPathProvider(options);
        var path = pluginPathProvider.GetPathToPlugins();

        path.ShouldBe("folder");
    }

    [Fact]
    public void GivenNoInstallationFolder_WhenGetPluginPath_ThenReturnsDefaultPath()
    {
        var options = Options.Create(new PluginsSettings());

        var pluginPathProvider = new PluginPathProvider(options);
        var path = pluginPathProvider.GetPathToPlugins();

        path.ShouldBe("/app/plugins");
    }
}
