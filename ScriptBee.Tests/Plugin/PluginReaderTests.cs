using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Moq;
using ScriptBee.FileManagement;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBee.Tests.Plugin.Internals;
using Serilog;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginReaderTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Fixture _fixture;
    private readonly Mock<ILogger> _loggerMock;

    private readonly PluginReader _pluginReader;
    private readonly Mock<IPluginManifestYamlFileReader> _yamlFileReaderMock;

    public PluginReaderTests()
    {
        _loggerMock = new Mock<ILogger>();
        _fileServiceMock = new Mock<IFileService>();
        _yamlFileReaderMock = new Mock<IPluginManifestYamlFileReader>();
        _fixture = new Fixture();

        _pluginReader = new PluginReader(_loggerMock.Object, _fileServiceMock.Object, _yamlFileReaderMock.Object);
    }

    [Fact]
    public void GivenNoPluginsInFolder_WhenReadPlugins_ThenReturnEmptyList()
    {
        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new List<string>());

        var result = _pluginReader.ReadPlugins("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenFolderWithNoManifest_WhenReadPlugins_ThenReturnEmptyList()
    {
        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new List<string> { "path" });
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

        var result = _pluginReader.ReadPlugins("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenErrorWhileReadingManifest_WhenReadPlugins_ThenReturnEmptyList()
    {
        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new List<string> { "path" });
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        _yamlFileReaderMock.Setup(x => x.Read(It.IsAny<string>())).Throws(new Exception());

        var result = _pluginReader.ReadPlugins("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenValidManifests_WhenReadPlugins_ThenReturnManifests()
    {
        var pluginExtensionPoint1 = _fixture.Create<ScriptGeneratorPluginExtensionPoint>();
        var pluginExtensionPoint2 = _fixture.Create<UiPluginExtensionPoint>();

        var pluginManifest1 = _fixture.Build<PluginManifest>()
            .With(p => p.ExtensionPoints, new List<PluginExtensionPoint> { pluginExtensionPoint1 })
            .Create();
        var pluginManifest2 = _fixture.Build<PluginManifest>()
            .With(p => p.ExtensionPoints, new List<PluginExtensionPoint> { pluginExtensionPoint2 })
            .Create();

        const string path1ManifestYaml = "path1/manifest.yaml";
        const string path2ManifestYaml = "path2/manifest.yaml";

        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>()))
            .Returns(new List<string> { "path1", "path2" });
        _fileServiceMock.Setup(x => x.CombinePaths("path1", "manifest.yaml")).Returns(path1ManifestYaml);
        _fileServiceMock.Setup(x => x.CombinePaths("path2", "manifest.yaml")).Returns(path2ManifestYaml);
        _fileServiceMock.Setup(x => x.FileExists(path1ManifestYaml)).Returns(true);
        _fileServiceMock.Setup(x => x.FileExists(path2ManifestYaml)).Returns(true);
        _yamlFileReaderMock.Setup(x => x.Read(path1ManifestYaml)).Returns(pluginManifest1);
        _yamlFileReaderMock.Setup(x => x.Read(path2ManifestYaml)).Returns(pluginManifest2);

        var result = _pluginReader.ReadPlugins("path").ToList();

        Assert.Equal(2, result.Count);
    }
}
