using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Web.Config;
using ScriptBee.Web.Services;

namespace ScriptBee.Web.Tests.Services;

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

    [Fact]
    public void GetPathToPlugins()
    {
        var options = Options.Create(new PluginsSettings());

        var pluginPathProvider = new PluginPathProvider(options);
        var path = pluginPathProvider.GetPathToPlugins(ProjectId.FromValue("project-id"));

        path.ShouldBe(Path.Combine(ConfigFolders.PathToProjects, "project-id", "plugins"));
    }
}
