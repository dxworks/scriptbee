using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Persistence.File.Plugin;

namespace ScriptBee.Persistence.File.Tests.Plugin;

public class PluginReaderTests
{
    private readonly ILogger<PluginReader> _logger = Substitute.For<ILogger<PluginReader>>();
    private readonly IFileService _fileService = Substitute.For<IFileService>();

    private readonly IPluginManifestYamlFileReader _pluginManifestYamlFileReader =
        Substitute.For<IPluginManifestYamlFileReader>();

    private readonly PluginReader _pluginReader;

    public PluginReaderTests()
    {
        _pluginReader = new PluginReader(_logger, _fileService, _pluginManifestYamlFileReader);
    }

    [Fact]
    public void GivenNoPluginsInFolder_WhenReadPlugins_ThenReturnEmptyList()
    {
        _fileService.GetDirectories(Arg.Any<string>()).Returns(new List<string>());

        var result = _pluginReader.ReadPlugins("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenFolderWithNoManifest_WhenReadPlugins_ThenReturnEmptyList()
    {
        _fileService.GetDirectories(Arg.Any<string>()).Returns(new List<string> { "path" });
        _fileService.FileExists(Arg.Any<string>()).Returns(false);

        var result = _pluginReader.ReadPlugins("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenErrorWhileReadingManifest_WhenReadPlugins_ThenReturnEmptyList()
    {
        _fileService.GetDirectories(Arg.Any<string>()).Returns(new List<string> { "path" });
        _fileService.FileExists(Arg.Any<string>()).Returns(true);
        _pluginManifestYamlFileReader.When(x => x.Read(Arg.Any<string>())).Throws(new Exception());

        var result = _pluginReader.ReadPlugins("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenValidManifests_WhenReadPlugins_ThenReturnManifests()
    {
        const string path1ManifestYaml = "path1/manifest.yaml";
        const string path2ManifestYaml = "path2/manifest.yaml";
        var pluginExtensionPoint1 = new ScriptGeneratorPluginExtensionPoint();
        var pluginExtensionPoint2 = new UiPluginExtensionPoint();
        var pluginManifest1 = new PluginManifest { ExtensionPoints = [pluginExtensionPoint1] };
        var pluginManifest2 = new PluginManifest { ExtensionPoints = [pluginExtensionPoint2] };
        _fileService
            .GetDirectories(Arg.Any<string>())
            .Returns(new List<string> { "path1", "path2" });
        _fileService.CombinePaths("path1", "manifest.yaml").Returns(path1ManifestYaml);
        _fileService.CombinePaths("path2", "manifest.yaml").Returns(path2ManifestYaml);
        _fileService.FileExists(path1ManifestYaml).Returns(true);
        _fileService.FileExists(path2ManifestYaml).Returns(true);
        _fileService.GetFileName("path1").Returns("path1@0.0.0");
        _fileService.GetFileName("path2").Returns("path2@0.0.0");
        _pluginManifestYamlFileReader.Read(path1ManifestYaml).Returns(pluginManifest1);
        _pluginManifestYamlFileReader.Read(path2ManifestYaml).Returns(pluginManifest2);

        var result = _pluginReader.ReadPlugins("path");

        Assert.Equal(2, result.Count());
    }
}
