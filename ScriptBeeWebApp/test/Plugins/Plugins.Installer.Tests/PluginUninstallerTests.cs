using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Tests.Common;

namespace ScriptBee.Plugins.Installer.Tests;

public class PluginUninstallerTests : IClassFixture<TempDirFixture>
{
    private const string MarkedForDeleteFile = "__delete.txt";

    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();

    private readonly ILogger<PluginUninstaller> _logger = Substitute.For<
        ILogger<PluginUninstaller>
    >();

    private readonly PluginUninstaller _pluginUninstaller;
    private readonly TempDirFixture _fixture;

    public PluginUninstallerTests(TempDirFixture fixture)
    {
        _fixture = fixture;
        _pluginUninstaller = new PluginUninstaller(_pluginPathProvider, _logger);
    }

    [Fact]
    public void GivenPath_WhenForceUninstall_TheDirectoryIsDeleted()
    {
        var pluginPath = _fixture.CreateSubFolder("plugin_to_delete");

        _pluginUninstaller.ForceUninstall(pluginPath);

        Directory.Exists(pluginPath).ShouldBeFalse();
    }

    [Fact]
    public void GivenPath_WhenUninstall_ThenPathIsMarkedForDelete()
    {
        var pluginsPath = _fixture.CreateSubFolder("plugins_root");
        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);
        var deleteFilePath = Path.Combine(pluginsPath, MarkedForDeleteFile);

        _pluginUninstaller.Uninstall("path_to_plugin");

        File.Exists(deleteFilePath).ShouldBeTrue();
        File.ReadAllLines(deleteFilePath).ShouldContain("path_to_plugin");
    }

    [Fact]
    public void GivenNoDeleteFile_WhenDeleteMarkedPlugins_ThenNoPluginsAreDeleted()
    {
        var pluginsPath = _fixture.CreateSubFolder("empty_delete");
        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);

        _pluginUninstaller.DeleteMarkedPlugins();
    }

    [Fact]
    public void GivenNoPluginsToDelete_WhenDeleteMarkedPlugins_ThenMarkedToDeleteFileIsDeleted()
    {
        var pluginsPath = _fixture.CreateSubFolder("no_plugins_to_delete");
        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);
        var deleteFilePath = Path.Combine(pluginsPath, MarkedForDeleteFile);
        File.WriteAllText(deleteFilePath, "");

        _pluginUninstaller.DeleteMarkedPlugins();

        File.Exists(deleteFilePath).ShouldBeFalse();
    }

    [Fact]
    public void GivenOnePluginToDelete_WhenDeleteMarkedPlugins_ThenPluginFolderIsDeleted()
    {
        var pluginsPath = _fixture.CreateSubFolder("delete_one");
        var pluginPath = Path.Combine(pluginsPath, "plugin1");
        Directory.CreateDirectory(pluginPath);
        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);

        var deleteFilePath = Path.Combine(pluginsPath, MarkedForDeleteFile);
        File.WriteAllLines(deleteFilePath, [pluginPath]);

        _pluginUninstaller.DeleteMarkedPlugins();

        Directory.Exists(pluginPath).ShouldBeFalse();
        File.Exists(deleteFilePath).ShouldBeFalse();
    }

    [Fact]
    public void GivenMultiplePluginsToDelete_WhenDeleteMarkedPlugins_ThenAllFoldersAreDeleted()
    {
        var pluginsPath = _fixture.CreateSubFolder("delete_multiple");
        var pluginPath1 = Path.Combine(pluginsPath, "plugin1");
        var pluginPath2 = Path.Combine(pluginsPath, "plugin2");
        Directory.CreateDirectory(pluginPath1);
        Directory.CreateDirectory(pluginPath2);

        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);
        var deleteFilePath = Path.Combine(pluginsPath, MarkedForDeleteFile);
        File.WriteAllLines(deleteFilePath, [pluginPath1, pluginPath2]);

        _pluginUninstaller.DeleteMarkedPlugins();

        Directory.Exists(pluginPath1).ShouldBeFalse();
        Directory.Exists(pluginPath2).ShouldBeFalse();
        File.Exists(deleteFilePath).ShouldBeFalse();
    }
}
