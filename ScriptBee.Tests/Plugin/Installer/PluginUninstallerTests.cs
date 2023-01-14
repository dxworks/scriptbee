using System;
using System.Collections.Generic;
using Moq;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Installer;
using Serilog;
using Xunit;

namespace ScriptBee.Tests.Plugin.Installer;

public class PluginUninstallerTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly PluginUninstaller _pluginUninstaller;
    private const string MarkedForDeleteFile = "__delete.txt";

    public PluginUninstallerTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        _loggerMock = new Mock<ILogger>();

        _pluginUninstaller = new PluginUninstaller(_fileServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GivenPath_WhenForceUninstall_TheDirectoryIsDeleted()
    {
        _pluginUninstaller.ForceUninstall("path");

        _fileServiceMock.Verify(x => x.DeleteDirectory("path"), Times.Once());
    }

    [Fact]
    public void GivenPath_WhenUninstall_ThenPathIsMarkedForDelete()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, MarkedForDeleteFile))
            .Returns("delete_path");

        _pluginUninstaller.Uninstall("path_to_plugin");

        _fileServiceMock.Verify(x => x.AppendTextToFile("delete_path", "path_to_plugin"), Times.Once());
    }

    [Fact]
    public void GivenNoDeleteFile_WhenDeleteMarkedPlugins_ThenNoPluginsAreDeleted()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, MarkedForDeleteFile))
            .Returns("delete_path");
        _fileServiceMock.Setup(x => x.FileExists("delete_path")).Returns(false);

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteDirectory(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public void GivenNoPluginsToDelete_WhenDeleteMarkedPlugins_ThenNoDirectoryIsDeleted()
    {
        SetupPluginsToDelete(Array.Empty<string>());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteDirectory(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public void
        GivenOnePluginAndThrowsExceptionWhileDeleting_WhenDeleteMarkedPlugins_ThenFolderWithProblemIsNotDeleted()
    {
        SetupPluginsToDelete(new[] { "path_to_plugin" });
        _fileServiceMock.Setup(x => x.DeleteDirectory("path_to_plugin")).Throws(new Exception());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteDirectory("path_to_plugin"), Times.Once());
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin"),
            Times.Once());
    }

    [Fact]
    public void GivenOnePluginToDelete_WhenDeleteMarkedPlugins_ThenPluginFolderIsDeleted()
    {
        SetupPluginsToDelete(new[] { "path_to_plugin" });

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteDirectory("path_to_plugin"), Times.Once());
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin"),
            Times.Never());
    }

    [Fact]
    public void GivenMultiplePluginsAndAllThrowExceptionWhileDeleting_WhenDeleteMarkedPlugins_ThenNoFolderIsDeleted()
    {
        SetupPluginsToDelete(new[] { "path_to_plugin1", "path_to_plugin2", "path_to_plugin3" });
        _fileServiceMock.Setup(x => x.DeleteDirectory("path_to_plugin1")).Throws(new Exception());
        _fileServiceMock.Setup(x => x.DeleteDirectory("path_to_plugin2")).Throws(new Exception());
        _fileServiceMock.Setup(x => x.DeleteDirectory("path_to_plugin3")).Throws(new Exception());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteDirectory(It.IsAny<string>()), Times.Exactly(3));
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin1"),
            Times.Once());
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin2"),
            Times.Once());
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin3"),
            Times.Once());
    }

    [Fact]
    public void GivenMultiplePluginsAndNoThrowExceptionWhileDeleting_WhenDeleteMarkedPlugins_ThenAllFoldersAreDeleted()
    {
        SetupPluginsToDelete(new[] { "path_to_plugin1", "path_to_plugin2", "path_to_plugin3" });

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteDirectory(It.IsAny<string>()), Times.Exactly(3));
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin1"),
            Times.Never());
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin2"),
            Times.Never());
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin3"),
            Times.Never());
    }

    [Fact]
    public void
        GivenMultiplePluginsAndSomeExceptionWhileDeleting_WhenDeleteMarkedPlugins_ThenFolderWithProblemIsNotDeleted()
    {
        SetupPluginsToDelete(new[] { "path_to_plugin1", "path_to_plugin2", "path_to_plugin3" });
        _fileServiceMock.Setup(x => x.DeleteDirectory("path_to_plugin2")).Throws(new Exception());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteDirectory(It.IsAny<string>()), Times.Exactly(3));
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin1"),
            Times.Never());
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin2"),
            Times.Once());
        _loggerMock.Verify(
            x => x.Error(It.IsAny<Exception>(), "Error deleting plugin '{PluginToDelete};", "path_to_plugin3"),
            Times.Never());
    }

    [Fact]
    public void GivenNoPluginsToDelete_WhenDeleteMarkedPlugins_ThenMarkedToDeleteFileIsDeleted()
    {
        SetupPluginsToDelete(Array.Empty<string>());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteFile("delete_path"), Times.Once());
    }

    [Fact]
    public void GivenMultiplePluginsWithNoError_WhenDeleteMarkedPlugins_ThenMarkedToDeleteFileIsDeleted()
    {
        SetupPluginsToDelete(new[] { "path_to_plugin1", "path_to_plugin2", "path_to_plugin3" });

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteFile("delete_path"), Times.Once());
    }

    [Fact]
    public void GivenMultiplePluginsWithSomeError_WhenDeleteMarkedPlugins_ThenMarkedToDeleteFileIsNotDeleted()
    {
        SetupPluginsToDelete(new[] { "path_to_plugin1", "path_to_plugin2", "path_to_plugin3" });
        _fileServiceMock.Setup(x => x.DeleteDirectory("path_to_plugin2")).Throws(new Exception());

        _pluginUninstaller.DeleteMarkedPlugins();

        _fileServiceMock.Verify(x => x.DeleteFile("delete_path"), Times.Never());
    }

    private void SetupPluginsToDelete(IEnumerable<string> pluginPaths)
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, MarkedForDeleteFile))
            .Returns("delete_path");
        _fileServiceMock.Setup(x => x.FileExists("delete_path")).Returns(true);
        _fileServiceMock.Setup(x => x.ReadAllLines("delete_path")).Returns(pluginPaths);
    }
}
