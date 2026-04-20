using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Analysis;
using ScriptBee.Service.Analysis.Config;

namespace ScriptBee.Analysis.Service.Tests;

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

    [Fact]
    public void GetPathToPlugins()
    {
        var options = Options.Create(new PluginsSettings());

        var pluginPathProvider = new PluginPathProvider(options);
        var path = pluginPathProvider.GetPathToPlugins(ProjectId.FromValue("project-id"));

        path.ShouldBe(Path.Combine(ConfigFolders.PathToProjects, "project-id", "plugins"));
    }
}
