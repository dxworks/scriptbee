using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ScriptBee.Plugins.Installer.Tests;

public class PluginUninstallerTests
{
    private const string MarkedForDeleteFile = "__delete.txt";

    private readonly IFileService _fileService = Substitute.For<IFileService>();

    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();

    private readonly ILogger<PluginUninstaller> _logger = Substitute.For<
        ILogger<PluginUninstaller>
    >();

    private readonly PluginUninstaller _pluginUninstaller;

    public PluginUninstallerTests()
    {
        _pluginUninstaller = new PluginUninstaller(_fileService, _pluginPathProvider, _logger);
    }

    [Fact]
    public void GivenPath_WhenForceUninstall_TheDirectoryIsDeleted()
    {
        _pluginUninstaller.ForceUninstall("path");

        _fileService.Received(1).DeleteDirectory("path");
    }

    [Fact]
    public void GivenPath_WhenUninstall_ThenPathIsMarkedForDelete()
    {
        _pluginPathProvider.GetPathToPlugins().Returns("plugin/path");
        _fileService.CombinePaths("plugin/path", MarkedForDeleteFile).Returns("delete_path");

        _pluginUninstaller.Uninstall("path_to_plugin");

        _fileService.Received(1).AppendTextToFile("delete_path", "path_to_plugin");
    }

    [Fact]
    public void GivenNoDeleteFile_WhenDeleteMarkedPlugins_ThenNoPluginsAreDeleted()
    {
        _pluginPathProvider.GetPathToPlugins().Returns("plugin/path");
        _fileService.CombinePaths("plugin/path", MarkedForDeleteFile).Returns("delete_path");
        _fileService.FileExists("delete_path").Returns(false);

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(0).DeleteDirectory(Arg.Any<string>());
    }

    [Fact]
    public void GivenNoPluginsToDelete_WhenDeleteMarkedPlugins_ThenNoDirectoryIsDeleted()
    {
        SetupPluginsToDelete([]);

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(0).DeleteDirectory(Arg.Any<string>());
    }

    [Fact]
    public void GivenOnePluginAndThrowsExceptionWhileDeleting_WhenDeleteMarkedPlugins_ThenFolderWithProblemIsNotDeleted()
    {
        SetupPluginsToDelete(["path_to_plugin"]);
        _fileService.When(x => x.DeleteDirectory("path_to_plugin")).Throws(new Exception());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(1).DeleteDirectory("path_to_plugin");
    }

    [Fact]
    public void GivenOnePluginToDelete_WhenDeleteMarkedPlugins_ThenPluginFolderIsDeleted()
    {
        SetupPluginsToDelete(["path_to_plugin"]);

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(1).DeleteDirectory("path_to_plugin");
    }

    [Fact]
    public void GivenMultiplePluginsAndAllThrowExceptionWhileDeleting_WhenDeleteMarkedPlugins_ThenNoFolderIsDeleted()
    {
        SetupPluginsToDelete(["path_to_plugin1", "path_to_plugin2", "path_to_plugin3"]);
        _fileService.When(x => x.DeleteDirectory("path_to_plugin1")).Throws(new Exception());
        _fileService.When(x => x.DeleteDirectory("path_to_plugin2")).Throws(new Exception());
        _fileService.When(x => x.DeleteDirectory("path_to_plugin3")).Throws(new Exception());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(3).DeleteDirectory(Arg.Any<string>());
    }

    [Fact]
    public void GivenMultiplePluginsAndNoThrowExceptionWhileDeleting_WhenDeleteMarkedPlugins_ThenAllFoldersAreDeleted()
    {
        SetupPluginsToDelete(["path_to_plugin1", "path_to_plugin2", "path_to_plugin3"]);

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(3).DeleteDirectory(Arg.Any<string>());
    }

    [Fact]
    public void GivenMultiplePluginsAndSomeExceptionWhileDeleting_WhenDeleteMarkedPlugins_ThenFolderWithProblemIsNotDeleted()
    {
        SetupPluginsToDelete(["path_to_plugin1", "path_to_plugin2", "path_to_plugin3"]);
        _fileService.When(x => x.DeleteDirectory("path_to_plugin2")).Throws(new Exception());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(3).DeleteDirectory(Arg.Any<string>());
    }

    [Fact]
    public void GivenNoPluginsToDelete_WhenDeleteMarkedPlugins_ThenMarkedToDeleteFileIsDeleted()
    {
        SetupPluginsToDelete([]);

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(1).DeleteFile("delete_path");
    }

    [Fact]
    public void GivenMultiplePluginsWithNoError_WhenDeleteMarkedPlugins_ThenMarkedToDeleteFileIsDeleted()
    {
        SetupPluginsToDelete(["path_to_plugin1", "path_to_plugin2", "path_to_plugin3"]);

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(1).DeleteFile("delete_path");
    }

    [Fact]
    public void GivenMultiplePluginsWithSomeError_WhenDeleteMarkedPlugins_ThenMarkedToDeleteFileIsNotDeleted()
    {
        SetupPluginsToDelete(["path_to_plugin1", "path_to_plugin2", "path_to_plugin3"]);
        _fileService.When(x => x.DeleteDirectory("path_to_plugin2")).Throws(new Exception());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileService.Received(0).DeleteFile("delete_path");
    }

    private void SetupPluginsToDelete(IEnumerable<string> pluginPaths)
    {
        _pluginPathProvider.GetPathToPlugins().Returns("plugin/path");
        _fileService.CombinePaths("plugin/path", MarkedForDeleteFile).Returns("delete_path");
        _fileService.FileExists("delete_path").Returns(true);
        _fileService.ReadAllLines("delete_path").Returns(pluginPaths);
    }
}
