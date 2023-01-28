using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Exceptions;
using ScriptBee.Plugin.Installer;
using ScriptBee.Services;
using Serilog;
using Xunit;

namespace ScriptBee.Tests.Plugin.Installer;

public class SimplePluginInstallerTests
{
    private readonly Mock<IDownloadService> _downloadServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IZipFileService> _zipFileServiceMock;
    private readonly SimplePluginInstaller _simplePluginInstaller;

    public SimplePluginInstallerTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        _zipFileServiceMock = new Mock<IZipFileService>();
        _downloadServiceMock = new Mock<IDownloadService>();
        var loggerMock = new Mock<ILogger>();

        _simplePluginInstaller = new SimplePluginInstaller(_fileServiceMock.Object, _zipFileServiceMock.Object,
            _downloadServiceMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GivenUrl_WhenInstall_ThenPluginsFolderPathIsCreated()
    {
        await _simplePluginInstaller.Install("url", "pluginName", "1.0.0", It.IsAny<CancellationToken>());

        _fileServiceMock.Verify(x => x.CreateFolder(ConfigFolders.PathToPlugins), Times.Once);
    }

    [Fact]
    public async Task GivenUrlDownloadException_WhenInstall_ThenNoZipFileIsPresent()
    {
        _downloadServiceMock.Setup(x =>
                x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0"))
            .Returns("path/pluginName@1.0.0");
        _fileServiceMock.Setup(x => x.FileExists("path/pluginName@1.0.0.zip"))
            .Returns(true);

        var pluginInstallationException = await Assert.ThrowsAsync<PluginInstallationException>(() =>
            _simplePluginInstaller.Install("url", "pluginName", "1.0.0", It.IsAny<CancellationToken>()));

        Assert.Equal("Plugin with name 'pluginName' and version '1.0.0' could not be installed.",
            pluginInstallationException.Message);
        _fileServiceMock.Verify(x => x.DeleteFile("path/pluginName@1.0.0.zip"), Times.Once());
        _fileServiceMock.Verify(x=>x.DeleteDirectory("path/pluginName@1.0.0"), Times.Once());
    }

    [Fact]
    public async Task GivenUrlDownloadExceptionAndNoZipFileIsDownloaded_WhenInstall_ThenNoZipFileIsPresent()
    {
        _downloadServiceMock.Setup(x =>
                x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0"))
            .Returns("path/pluginName@1.0.0");
        _fileServiceMock.Setup(x => x.FileExists("path/pluginName@1.0.0.zip"))
            .Returns(false);

        var pluginInstallationException = await Assert.ThrowsAsync<PluginInstallationException>(() =>
            _simplePluginInstaller.Install("url", "pluginName", "1.0.0", It.IsAny<CancellationToken>()));

        Assert.Equal("Plugin with name 'pluginName' and version '1.0.0' could not be installed.",
            pluginInstallationException.Message);
        _fileServiceMock.Verify(x => x.DeleteFile("path/pluginName@1.0.0.zip"), Times.Once());
        _fileServiceMock.Verify(x=>x.DeleteDirectory("path/pluginName@1.0.0"), Times.Once());
    }

    [Fact]
    public async Task GivenUnzipException_WhenInstall_ThenNoZipFileIsPresent()
    {
        _zipFileServiceMock.Setup(x =>
                x.UnzipFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception());
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0"))
            .Returns("path/pluginName@1.0.0");
        _fileServiceMock.Setup(x => x.FileExists("path/pluginName@1.0.0.zip"))
            .Returns(true);

        var pluginInstallationException = await Assert.ThrowsAsync<PluginInstallationException>(() =>
            _simplePluginInstaller.Install("url", "pluginName", "1.0.0", It.IsAny<CancellationToken>()));

        Assert.Equal("Plugin with name 'pluginName' and version '1.0.0' could not be installed.",
            pluginInstallationException.Message);
        _downloadServiceMock.Verify(
            x => x.DownloadFileAsync("url", "path/pluginName@1.0.0.zip", It.IsAny<CancellationToken>()), Times.Once());
        _fileServiceMock.Verify(x => x.DeleteFile("path/pluginName@1.0.0.zip"), Times.Once());
        _fileServiceMock.Verify(x=>x.DeleteDirectory("path/pluginName@1.0.0"), Times.Once());
    }

    [Fact]
    public async Task GivenUrlForPluginAndVersion_WhenInstall_ThenZipFileIsDownloadedSuccessful()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0"))
            .Returns("path/pluginName@1.0.0");

        await _simplePluginInstaller.Install("url", "pluginName", "1.0.0", It.IsAny<CancellationToken>());

        _downloadServiceMock.Verify(
            x => x.DownloadFileAsync("url", "path/pluginName@1.0.0.zip", It.IsAny<CancellationToken>()), Times.Once());
        _zipFileServiceMock.Verify(
            x => x.UnzipFileAsync("path/pluginName@1.0.0.zip", "path/pluginName@1.0.0", It.IsAny<CancellationToken>()),
            Times.Once());
        _fileServiceMock.Verify(x => x.DeleteFile("path/pluginName@1.0.0.zip"), Times.Once());
    }

    [Fact]
    public async Task GivenExistingPluginWithTheSameVersion_WhenInstall_ThenPluginIsNotInstalled()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0"))
            .Returns("path/pluginName@1.0.0");
        _fileServiceMock.Setup(x => x.DirectoryExists("path/pluginName@1.0.0"))
            .Returns(true);

        var pluginVersionExistsException = await Assert.ThrowsAsync<PluginVersionExistsException>(() =>
            _simplePluginInstaller.Install("url", "pluginName", "1.0.0", It.IsAny<CancellationToken>()));

        Assert.Equal("Plugin with name 'pluginName' and version '1.0.0' already exists",
            pluginVersionExistsException.Message);
        _downloadServiceMock.Verify(
            x => x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [Fact]
    public async Task GivenPluginUrl_WhenInstall_ThenPluginIsInstalledSuccessfully()
    {
        _fileServiceMock.Setup(x => x.CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0"))
            .Returns("path/pluginName@1.0.0");

        var installedPluginPath =
            await _simplePluginInstaller.Install("url", "pluginName", "1.0.0", It.IsAny<CancellationToken>());

        Assert.Equal("path/pluginName@1.0.0", installedPluginPath);
    }
}
