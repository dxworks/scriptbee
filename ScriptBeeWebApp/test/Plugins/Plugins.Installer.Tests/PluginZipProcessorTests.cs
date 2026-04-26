using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.Tests.Common.Plugins;

namespace ScriptBee.Plugins.Installer.Tests;

public class PluginZipProcessorTests : IClassFixture<TempDirFixture>
{
    private readonly IZipFileService _zipFileService = Substitute.For<IZipFileService>();
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();

    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();

    private readonly ILogger<PluginZipProcessor> _logger = Substitute.For<
        ILogger<PluginZipProcessor>
    >();

    private readonly PluginZipProcessor _pluginZipProcessor;
    private readonly ProjectId _projectId = ProjectId.FromValue("project-id");
    private readonly string _projectPluginsPath;
    private readonly string _globalPluginsPath;

    public PluginZipProcessorTests(TempDirFixture tempDirFixture)
    {
        _pluginZipProcessor = new PluginZipProcessor(
            _zipFileService,
            _pluginReader,
            _pluginPathProvider,
            _logger
        );

        _projectPluginsPath = tempDirFixture.CreateSubFolder($"project_plugins_{Guid.NewGuid()}");
        _globalPluginsPath = tempDirFixture.CreateSubFolder($"global_plugins_{Guid.NewGuid()}");

        _pluginPathProvider.GetPathToPlugins(_projectId).Returns(_projectPluginsPath);
        _pluginPathProvider.GetPathToPlugins().Returns(_globalPluginsPath);
    }

    [Fact]
    public async Task GivenZipStream_WhenManifestNotFound_ThenReturnsPluginManifestNotFoundError()
    {
        // Arrange
        _zipFileService
            .UnzipFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(ci =>
            {
                var tempFolder = ci.ArgAt<string>(1);
                Directory.CreateDirectory(tempFolder);
            });

        _pluginReader.ReadPlugin(Arg.Any<string>()).Returns((Plugin?)null);

        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var result = await _pluginZipProcessor.ProcessZipStream(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenZipStream_WhenPluginAlreadyExistsInProject_ThenReturnsPluginId()
    {
        // Arrange
        var pluginId = new PluginId("plugin-name", new Version("1.0.0"));
        var testPlugin = new TestPlugin(pluginId);

        _zipFileService
            .UnzipFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(ci =>
            {
                var tempFolder = ci.ArgAt<string>(1);
                Directory.CreateDirectory(tempFolder);
            });

        _pluginReader.ReadPlugin(Arg.Any<string>()).Returns(testPlugin);

        var existingProjectPluginPath = Path.Combine(
            _projectPluginsPath,
            pluginId.GetFullyQualifiedName()
        );
        Directory.CreateDirectory(existingProjectPluginPath);

        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var result = await _pluginZipProcessor.ProcessZipStream(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(pluginId);
    }

    [Fact]
    public async Task GivenZipStream_WhenPluginAlreadyExistsGlobally_ThenReturnsPluginId()
    {
        // Arrange
        var pluginId = new PluginId("plugin-name", new Version("1.0.0"));
        var testPlugin = new TestPlugin(pluginId);

        _zipFileService
            .UnzipFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(ci =>
            {
                var tempFolder = ci.ArgAt<string>(1);
                Directory.CreateDirectory(tempFolder);
            });

        _pluginReader.ReadPlugin(Arg.Any<string>()).Returns(testPlugin);

        var existingGlobalPluginPath = Path.Combine(
            _globalPluginsPath,
            pluginId.GetFullyQualifiedName()
        );
        Directory.CreateDirectory(existingGlobalPluginPath);

        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var result = await _pluginZipProcessor.ProcessZipStream(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(pluginId);
    }

    [Fact]
    public async Task GivenZipStream_WhenExceptionThrown_ThenReturnsPluginInstallationError()
    {
        // Arrange
        _zipFileService
            .When(x =>
                x.UnzipFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            )
            .Throws(new Exception("Unzip simulated failure"));

        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var result = await _pluginZipProcessor.ProcessZipStream(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT2.ShouldBeTrue();
        result.AsT2.Id.Name.ShouldBe("Unknown");
        result.AsT2.Id.Version.ShouldBe(new Version("0.0.0"));

        _logger
            .Received()
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString()!.Contains("Error processing uploaded plugin")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
    }

    [Fact]
    public async Task GivenZipStream_WhenSuccessful_ThenReturnsPluginIdAndMovesFolder()
    {
        // Arrange
        var pluginId = new PluginId("plugin-name", new Version("1.0.0"));
        var testPlugin = new TestPlugin(pluginId);

        _zipFileService
            .UnzipFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(ci =>
            {
                var tempFolder = ci.ArgAt<string>(1);
                Directory.CreateDirectory(tempFolder);
                // Create a dummy file to prove the folder gets moved correctly
                File.WriteAllText(Path.Combine(tempFolder, "manifest.json"), "{}");
            });

        _pluginReader.ReadPlugin(Arg.Any<string>()).Returns(testPlugin);

        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var result = await _pluginZipProcessor.ProcessZipStream(
            _projectId,
            stream,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(pluginId);

        var expectedFinalPath = Path.Combine(_projectPluginsPath, pluginId.GetFullyQualifiedName());
        Directory.Exists(expectedFinalPath).ShouldBeTrue();
        File.Exists(Path.Combine(expectedFinalPath, "manifest.json")).ShouldBeTrue();

        // Ensure no lingering temporary zip files
        var tempZips = Directory.GetFiles(_projectPluginsPath, "*.zip");
        tempZips.ShouldBeEmpty();
    }
}
