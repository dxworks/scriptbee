using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Tests.Common;

namespace ScriptBee.Plugins.Tests;

public class PluginReaderTests : IClassFixture<TempDirFixture>
{
    private readonly ILogger<PluginReader> _logger = Substitute.For<ILogger<PluginReader>>();

    private readonly IPluginManifestYamlFileReader _pluginManifestYamlFileReader =
        Substitute.For<IPluginManifestYamlFileReader>();

    private readonly PluginReader _pluginReader;

    private readonly TempDirFixture _tempDirFixture;

    public PluginReaderTests(TempDirFixture fixture)
    {
        _tempDirFixture = fixture;
        _pluginReader = new PluginReader(_logger, _pluginManifestYamlFileReader);
    }

    [Fact]
    public void GivenPluginsDoesNotExists_WhenReadPlugins_ThenReturnEmptyList()
    {
        var result = _pluginReader.ReadPlugins("non-existing");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenNoPluginsInFolder_WhenReadPlugins_ThenReturnEmptyList()
    {
        var pluginsFolder = _tempDirFixture.CreateSubFolder("empty-plugins-folder");

        var result = _pluginReader.ReadPlugins(pluginsFolder);

        Assert.Empty(result);
    }

    [Fact]
    public void GivenFolderWithNoManifest_WhenReadPlugins_ThenReturnEmptyList()
    {
        var pluginsFolder = _tempDirFixture.CreateSubFolder("plugins-folder");
        var path = Path.Combine(pluginsFolder, "empty-plugin-folder");
        Directory.CreateDirectory(path);

        var result = _pluginReader.ReadPlugins(pluginsFolder);

        Assert.Empty(result);
    }

    [Fact]
    public void GivenErrorWhileReadingManifest_WhenReadPlugins_ThenReturnEmptyList()
    {
        // Arrange
        var pluginsFolder = _tempDirFixture.CreateSubFolder("plugins-folder");
        var pluginFolder = Path.Combine(pluginsFolder, "plugin-folder");
        Directory.CreateDirectory(pluginFolder);
        File.WriteAllText(Path.Combine(pluginFolder, "manifest.yaml"), "");

        _pluginManifestYamlFileReader.When(x => x.Read(Arg.Any<string>())).Throws(new Exception());

        // Act
        var result = _pluginReader.ReadPlugins(pluginsFolder);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GivenValidManifests_WhenReadPlugins_ThenReturnManifests()
    {
        // Arrange
        var pluginsFolder = _tempDirFixture.CreateSubFolder("plugins-folder");

        var pluginFolder1 = Path.Combine(pluginsFolder, "path1@0.0.0");
        Directory.CreateDirectory(pluginFolder1);
        var path1ManifestYaml = Path.Combine(pluginFolder1, "manifest.yaml");
        File.WriteAllText(path1ManifestYaml, "");

        var pluginFolder2 = Path.Combine(pluginsFolder, "path2@0.0.0");
        Directory.CreateDirectory(pluginFolder2);
        var path2ManifestYaml = Path.Combine(pluginFolder2, "manifest.yaml");
        File.WriteAllText(path2ManifestYaml, "");

        var pluginExtensionPoint1 = new ScriptGeneratorPluginExtensionPoint();
        var pluginExtensionPoint2 = new UiPluginExtensionPoint();
        var pluginManifest1 = new PluginManifest { ExtensionPoints = [pluginExtensionPoint1] };
        var pluginManifest2 = new PluginManifest { ExtensionPoints = [pluginExtensionPoint2] };

        _pluginManifestYamlFileReader.Read(path1ManifestYaml).Returns(pluginManifest1);
        _pluginManifestYamlFileReader.Read(path2ManifestYaml).Returns(pluginManifest2);

        // Act
        var result = _pluginReader.ReadPlugins(pluginsFolder);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GivenFolderNameIsPluginId_WhenReadPlugin_ThenReturnPluginWithIdFromFolderName()
    {
        // Arrange
        var pluginFolder = _tempDirFixture.CreateSubFolder("TestPlugin@1.2.3");
        var manifestYaml = Path.Combine(pluginFolder, "manifest.yaml");
        File.WriteAllText(manifestYaml, "");

        var pluginManifest = new PluginManifest { Name = "OtherName" };
        _pluginManifestYamlFileReader.Read(manifestYaml).Returns(pluginManifest);

        // Act
        var result = _pluginReader.ReadPlugin(pluginFolder);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestPlugin", result.Id.Name);
        Assert.Equal("1.2.3", result.Id.Version.ToString());
    }

    [Fact]
    public void GivenFolderNameIsNotPluginIdButManifestHasExtensionPoints_WhenReadPlugin_ThenReturnPluginWithIdFromManifest()
    {
        // Arrange
        var pluginFolder = _tempDirFixture.CreateSubFolder("invalid-folder-name");
        var manifestYaml = Path.Combine(pluginFolder, "manifest.yaml");
        File.WriteAllText(manifestYaml, "");

        var pluginManifest = new PluginManifest
        {
            Name = "ManifestPlugin",
            ExtensionPoints = [new ScriptGeneratorPluginExtensionPoint { Version = "2.0.0" }],
        };
        _pluginManifestYamlFileReader.Read(manifestYaml).Returns(pluginManifest);

        // Act
        var result = _pluginReader.ReadPlugin(pluginFolder);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ManifestPlugin", result.Id.Name);
        Assert.Equal("2.0.0", result.Id.Version.ToString());
    }

    [Fact]
    public void GivenInvalidFolderNameAndNoExtensionPoints_WhenReadPlugin_ThenReturnNull()
    {
        // Arrange
        var pluginFolder = _tempDirFixture.CreateSubFolder("invalid-folder-name");
        var manifestYaml = Path.Combine(pluginFolder, "manifest.yaml");
        File.WriteAllText(manifestYaml, "");

        var pluginManifest = new PluginManifest { Name = "ManifestPlugin", ExtensionPoints = [] };
        _pluginManifestYamlFileReader.Read(manifestYaml).Returns(pluginManifest);

        // Act
        var result = _pluginReader.ReadPlugin(pluginFolder);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GivenInvalidFolderNameAndInvalidManifestId_WhenReadPlugin_ThenReturnNull()
    {
        // Arrange
        var pluginFolder = _tempDirFixture.CreateSubFolder("invalid-folder-name");
        var manifestYaml = Path.Combine(pluginFolder, "manifest.yaml");
        File.WriteAllText(manifestYaml, "");

        var pluginManifest = new PluginManifest
        {
            Name = "ManifestPlugin",
            ExtensionPoints =
            [
                new ScriptGeneratorPluginExtensionPoint { Version = "not-a-version" },
            ],
        };
        _pluginManifestYamlFileReader.Read(manifestYaml).Returns(pluginManifest);

        // Act
        var result = _pluginReader.ReadPlugin(pluginFolder);

        // Assert
        Assert.Null(result);
    }
}
