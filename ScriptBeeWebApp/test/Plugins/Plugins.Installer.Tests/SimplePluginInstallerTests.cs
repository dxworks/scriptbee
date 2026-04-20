using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Tests.Common;

namespace ScriptBee.Plugins.Installer.Tests;

public class SimplePluginInstallerTests : IClassFixture<TempDirFixture>
{
    private readonly IDownloadService _downloadService = Substitute.For<IDownloadService>();
    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();
    private readonly IZipFileService _zipFileService = Substitute.For<IZipFileService>();

    private readonly ILogger<SimplePluginInstaller> _logger = Substitute.For<
        ILogger<SimplePluginInstaller>
    >();
    private readonly TempDirFixture _tempDirFixture;

    private readonly SimplePluginInstaller _simplePluginInstaller;

    public SimplePluginInstallerTests(TempDirFixture tempDirFixture)
    {
        _tempDirFixture = tempDirFixture;
        _simplePluginInstaller = new SimplePluginInstaller(
            _zipFileService,
            _downloadService,
            _pluginPathProvider,
            _logger
        );
    }

    [Fact]
    public async Task GivenUrl_WhenInstall_ThenPluginsFolderPathIsCreated()
    {
        var pluginsPath = _tempDirFixture.CreateSubFolder($"plugins_{Guid.NewGuid()}");
        var subFolder = Path.Combine(pluginsPath, "new-dir");

        _pluginPathProvider.GetPathToPlugins().Returns(subFolder);

        await _simplePluginInstaller.Install(
            "url",
            new PluginId("pluginName", new Version("1.0.0")),
            TestContext.Current.CancellationToken
        );

        Directory.Exists(subFolder).ShouldBeTrue();
    }

    [Fact]
    public async Task GivenUrlDownloadException_WhenInstall_ThenReturnsPluginInstallationError()
    {
        var pluginsPath = _tempDirFixture.CreateSubFolder($"plugins_{Guid.NewGuid()}");
        var pluginId = new PluginId("pluginName", new Version("1.0.0"));

        _downloadService
            .When(x =>
                x.DownloadFileAsync(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<CancellationToken>()
                )
            )
            .Throws(new Exception("Download failed"));

        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);

        var result = await _simplePluginInstaller.Install(
            "url",
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
        result.AsT1.Id.Name.ShouldBe("pluginName");
        result.AsT1.Id.Version.ShouldBe(new Version("1.0.0"));
    }

    [Fact]
    public async Task GivenUrlDownloadExceptionAndNoZipFileIsDownloaded_WhenInstall_ThenReturnsPluginInstallationError()
    {
        var pluginsPath = _tempDirFixture.CreateSubFolder($"plugins_{Guid.NewGuid()}");
        var pluginId = new PluginId("pluginName", new Version("1.0.0"));

        _downloadService
            .When(x =>
                x.DownloadFileAsync(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<CancellationToken>()
                )
            )
            .Throws(new Exception("Download failed"));

        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);

        var result = await _simplePluginInstaller.Install(
            "url",
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
        result.AsT1.Id.Name.ShouldBe("pluginName");
        result.AsT1.Id.Version.ShouldBe(new Version("1.0.0"));
        File.Exists(Path.Combine(pluginsPath, "pluginName@1.0.0.zip")).ShouldBeFalse();
    }

    [Fact]
    public async Task GivenUnzipException_WhenInstall_ThenReturnsPluginInstallationError()
    {
        var pluginsPath = _tempDirFixture.CreateSubFolder($"plugins_{Guid.NewGuid()}");
        var pluginId = new PluginId("pluginName", new Version("1.0.0"));

        _zipFileService
            .When(x =>
                x.UnzipFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            )
            .Throws(new Exception("Unzip failed"));

        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);

        var result = await _simplePluginInstaller.Install(
            "url",
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
        result.AsT1.Id.Name.ShouldBe("pluginName");
        result.AsT1.Id.Version.ShouldBe(new Version("1.0.0"));
        await _downloadService
            .Received(1)
            .DownloadFileAsync(
                "url",
                Path.Combine(pluginsPath, "pluginName@1.0.0.zip"),
                Arg.Any<CancellationToken>()
            );
        File.Exists(Path.Combine(pluginsPath, "pluginName@1.0.0.zip")).ShouldBeFalse();
    }

    [Fact]
    public async Task GivenUrlForPluginAndVersion_WhenInstall_ThenZipFileIsDownloadedSuccessfully()
    {
        var pluginsPath = _tempDirFixture.CreateSubFolder($"plugins_{Guid.NewGuid()}");
        var pluginId = new PluginId("pluginName", new Version("1.0.0"));
        var expectedPluginPath = Path.Combine(pluginsPath, "pluginName@1.0.0");
        var expectedZipPath = Path.Combine(pluginsPath, "pluginName@1.0.0.zip");

        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);

        var result = await _simplePluginInstaller.Install(
            "url",
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(expectedPluginPath);
        await _downloadService
            .Received(1)
            .DownloadFileAsync("url", expectedZipPath, Arg.Any<CancellationToken>());
        await _zipFileService
            .Received(1)
            .UnzipFileAsync(expectedZipPath, expectedPluginPath, Arg.Any<CancellationToken>());
        File.Exists(expectedZipPath).ShouldBeFalse();
    }

    [Fact]
    public async Task GivenExistingPluginWithTheSameVersion_WhenInstall_ThenReturnSuccess()
    {
        var pluginsPath = _tempDirFixture.CreateSubFolder($"plugins_{Guid.NewGuid()}");
        var pluginId = new PluginId("pluginName", new Version("1.0.0"));
        var existingPluginPath = Path.Combine(pluginsPath, "pluginName@1.0.0");
        Directory.CreateDirectory(existingPluginPath);

        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);

        var result = await _simplePluginInstaller.Install(
            "url",
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(existingPluginPath);
        await _downloadService
            .Received(0)
            .DownloadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenPluginUrl_WhenInstall_ThenPluginIsInstalledSuccessfully()
    {
        var pluginsPath = _tempDirFixture.CreateSubFolder($"plugins_{Guid.NewGuid()}");
        var pluginId = new PluginId("pluginName", new Version("1.0.0"));
        var existingPluginPath = Path.Combine(pluginsPath, "pluginName@1.0.0");

        _pluginPathProvider.GetPathToPlugins().Returns(pluginsPath);

        var result = await _simplePluginInstaller.Install(
            "url",
            pluginId,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(existingPluginPath);
    }
}
