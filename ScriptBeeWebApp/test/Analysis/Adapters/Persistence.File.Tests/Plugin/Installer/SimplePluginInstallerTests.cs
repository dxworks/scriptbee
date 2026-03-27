using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Persistence.File.Plugin.Installer;

namespace ScriptBee.Persistence.File.Tests.Plugin.Installer;

public class SimplePluginInstallerTests
{
    private readonly IDownloadService _downloadService = Substitute.For<IDownloadService>();
    private readonly IFileService _fileService = Substitute.For<IFileService>();
    private readonly IZipFileService _zipFileService = Substitute.For<IZipFileService>();

    private readonly ILogger<SimplePluginInstaller> _logger = Substitute.For<
        ILogger<SimplePluginInstaller>
    >();

    private readonly SimplePluginInstaller _simplePluginInstaller;

    public SimplePluginInstallerTests()
    {
        _simplePluginInstaller = new SimplePluginInstaller(
            _fileService,
            _zipFileService,
            _downloadService,
            _logger
        );
    }

    [Fact]
    public async Task GivenUrl_WhenInstall_ThenPluginsFolderPathIsCreated()
    {
        await _simplePluginInstaller.Install(
            "url",
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        _fileService.Received(1).CreateFolder(ConfigFolders.PathToPlugins);
    }

    [Fact]
    public async Task GivenUrlDownloadException_WhenInstall_ThenReturnsPluginInstallationError()
    {
        _downloadService
            .When(x =>
                x.DownloadFileAsync(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<CancellationToken>()
                )
            )
            .Throws(new Exception());
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0")
            .Returns("path/pluginName@1.0.0");
        _fileService.FileExists("path/pluginName@1.0.0.zip").Returns(true);

        var result = await _simplePluginInstaller.Install(
            "url",
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBe(true);
        var error = result.AsT2;
        error.Name.ShouldBe("pluginName");
        error.Version.ShouldBe("1.0.0");
        _fileService.Received(1).DeleteFile("path/pluginName@1.0.0.zip");
        _fileService.Received(1).DeleteDirectory("path/pluginName@1.0.0");
    }

    [Fact]
    public async Task GivenUrlDownloadExceptionAndNoZipFileIsDownloaded_WhenInstall_ThenReturnsPluginInstallationError()
    {
        _downloadService
            .When(x =>
                x.DownloadFileAsync(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<CancellationToken>()
                )
            )
            .Throws(new Exception());
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0")
            .Returns("path/pluginName@1.0.0");
        _fileService.FileExists("path/pluginName@1.0.0.zip").Returns(false);

        var result = await _simplePluginInstaller.Install(
            "url",
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBe(true);
        var error = result.AsT2;
        error.Name.ShouldBe("pluginName");
        error.Version.ShouldBe("1.0.0");
        _fileService.Received(1).DeleteFile("path/pluginName@1.0.0.zip");
        _fileService.Received(1).DeleteDirectory("path/pluginName@1.0.0");
    }

    [Fact]
    public async Task GivenUnzipException_WhenInstall_ThenReturnsPluginInstallationError()
    {
        _zipFileService
            .When(x =>
                x.UnzipFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            )
            .Throws(new Exception());
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0")
            .Returns("path/pluginName@1.0.0");
        _fileService.FileExists("path/pluginName@1.0.0.zip").Returns(true);

        var result = await _simplePluginInstaller.Install(
            "url",
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT2.ShouldBe(true);
        var error = result.AsT2;
        error.Name.ShouldBe("pluginName");
        error.Version.ShouldBe("1.0.0");
        await _downloadService
            .Received(1)
            .DownloadFileAsync("url", "path/pluginName@1.0.0.zip", Arg.Any<CancellationToken>());
        _fileService.Received(1).DeleteFile("path/pluginName@1.0.0.zip");
        _fileService.Received(1).DeleteDirectory("path/pluginName@1.0.0");
    }

    [Fact]
    public async Task GivenUrlForPluginAndVersion_WhenInstall_ThenZipFileIsDownloadedSuccessfully()
    {
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0")
            .Returns("path/pluginName@1.0.0");

        var result = await _simplePluginInstaller.Install(
            "url",
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.ShouldBe("path/pluginName@1.0.0");
        await _downloadService
            .Received(1)
            .DownloadFileAsync("url", "path/pluginName@1.0.0.zip", Arg.Any<CancellationToken>());
        await _zipFileService
            .Received(1)
            .UnzipFileAsync(
                "path/pluginName@1.0.0.zip",
                "path/pluginName@1.0.0",
                Arg.Any<CancellationToken>()
            );
        _fileService.Received(1).DeleteFile("path/pluginName@1.0.0.zip");
    }

    [Fact]
    public async Task GivenExistingPluginWithTheSameVersion_WhenInstall_ThenReturnsPluginVersionExistsError()
    {
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0")
            .Returns("path/pluginName@1.0.0");
        _fileService.DirectoryExists("path/pluginName@1.0.0").Returns(true);

        var result = await _simplePluginInstaller.Install(
            "url",
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBe(true);
        var error = result.AsT1;
        error.Name.ShouldBe("pluginName");
        error.Version.ShouldBe("1.0.0");
        await _downloadService
            .Received(0)
            .DownloadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenPluginUrl_WhenInstall_ThenPluginIsInstalledSuccessfully()
    {
        _fileService
            .CombinePaths(ConfigFolders.PathToPlugins, "pluginName@1.0.0")
            .Returns("path/pluginName@1.0.0");

        var result = await _simplePluginInstaller.Install(
            "url",
            "pluginName",
            "1.0.0",
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBe(true);
        result.AsT0.ShouldBe("path/pluginName@1.0.0");
    }
}
