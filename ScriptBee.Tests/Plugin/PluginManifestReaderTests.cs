using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Moq;
using ScriptBee.FileManagement;
using ScriptBee.Plugin;
using Serilog;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginManifestReaderTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IYamlFileReader> _yamlFileReaderMock;
    private readonly Mock<IPluginManifestValidator> _pluginManifestValidatorMock;
    private readonly Fixture _fixture;

    private readonly PluginManifestReader _pluginManifestReader;

    public PluginManifestReaderTests()
    {
        var loggerMock = new Mock<ILogger>();
        _fileServiceMock = new Mock<IFileService>();
        _yamlFileReaderMock = new Mock<IYamlFileReader>();
        _pluginManifestValidatorMock = new Mock<IPluginManifestValidator>();
        _fixture = new Fixture();

        _pluginManifestReader =
            new PluginManifestReader(loggerMock.Object, _fileServiceMock.Object, _yamlFileReaderMock.Object,
                _pluginManifestValidatorMock.Object);
    }

    [Fact]
    public void GivenNoPluginsInFolder_WhenReadManifests_ThenReturnEmptyList()
    {
        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new List<string>());

        var result = _pluginManifestReader.ReadManifests("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenFolderWithNoManifest_WhenReadManifests_ThenReturnEmptyList()
    {
        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new List<string> { "path" });
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

        var result = _pluginManifestReader.ReadManifests("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenErrorWhileReadingManifest_WhenReadManifests_ThenReturnEmptyList()
    {
        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new List<string> { "path" });
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        _yamlFileReaderMock.Setup(x => x.Read<PluginManifest>(It.IsAny<string>())).Throws(new Exception());

        var result = _pluginManifestReader.ReadManifests("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenInvalidManifest_WhenReadManifests_ThenReturnEmptyList()
    {
        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new List<string> { "path" });
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        _pluginManifestValidatorMock.Setup(x => x.Validate(It.IsAny<PluginManifest>())).Returns(false);

        var result = _pluginManifestReader.ReadManifests("path");

        Assert.Empty(result);
    }

    [Fact]
    public void GivenValidManifests_WhenReadManifests_ThenReturnManifests()
    {
        var pluginManifest1 = _fixture.Create<PluginManifest>();
        var pluginManifest2 = _fixture.Create<PluginManifest>();

        const string path1ManifestYaml = "path1/manifest.yaml";
        const string path2ManifestYaml = "path2/manifest.yaml";

        _fileServiceMock.Setup(x => x.GetDirectories(It.IsAny<string>()))
            .Returns(new List<string> { "path1", "path2" });
        _fileServiceMock.Setup(x => x.CombinePaths("path1", "manifest.yaml")).Returns(path1ManifestYaml);
        _fileServiceMock.Setup(x => x.CombinePaths("path2", "manifest.yaml")).Returns(path2ManifestYaml);
        _fileServiceMock.Setup(x => x.FileExists(path1ManifestYaml)).Returns(true);
        _fileServiceMock.Setup(x => x.FileExists(path2ManifestYaml)).Returns(true);
        _yamlFileReaderMock.Setup(x => x.Read<PluginManifest>(path1ManifestYaml)).Returns(pluginManifest1);
        _yamlFileReaderMock.Setup(x => x.Read<PluginManifest>(path2ManifestYaml)).Returns(pluginManifest2);
        _pluginManifestValidatorMock.Setup(x => x.Validate(pluginManifest1)).Returns(true);
        _pluginManifestValidatorMock.Setup(x => x.Validate(pluginManifest2)).Returns(true);

        var result = _pluginManifestReader.ReadManifests("path").ToList();

        Assert.Equal(2, result.Count);
    }
}
