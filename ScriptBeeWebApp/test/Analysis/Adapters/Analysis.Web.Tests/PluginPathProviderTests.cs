using Microsoft.Extensions.Options;
using ScriptBee.Analysis.Web.Config;

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
    public void GivenNoInstallationFolder_WhenGetPluginPath_ThenReturnUserFolderPath()
    {
        var options = Options.Create(new PluginsSettings { InstallationFolder = null });

        var pluginPathProvider = new PluginPathProvider(options);
        var path = pluginPathProvider.GetPathToPlugins();

        path.ShouldBe(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".scriptbee",
                "plugins"
            )
        );
    }
}
