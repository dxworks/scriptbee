using ScriptBee.FileManagement;
using ScriptBee.Plugin.Manifest;
using Xunit;

namespace ScriptBee.Tests.Plugin.Manifest;

public class ScriptGeneratorPluginManifestYamlParsingTests
{
    private readonly PluginManifestYamlFileReader _yamlFileReader;

    public ScriptGeneratorPluginManifestYamlParsingTests()
    {
        _yamlFileReader = new PluginManifestYamlFileReader(new PluginDiscriminatorHolder());
    }

    [Theory]
    [FilePath("TestData/ScriptGeneratorPluginManifest.yaml")]
    public void GivenManifestContent_ThenScriptGeneratorPluginIsConstructed(string filePath)
    {
        var pluginManifest = (ScriptGeneratorPluginManifest)_yamlFileReader.Read(filePath);

        Assert.Equal("v1", pluginManifest.ApiVersion);
        Assert.Equal("ScriptGenerator", pluginManifest.Kind);
        Assert.Equal("ScriptBee", pluginManifest.Metadata.Author);
        Assert.Equal("Description", pluginManifest.Metadata.Description);
        Assert.Equal("Plugin.dll", pluginManifest.Metadata.EntryPoint);
        Assert.Equal("ScriptGenerator example", pluginManifest.Metadata.Name);
        Assert.Equal("0.0.1", pluginManifest.Metadata.Version);
        Assert.Null(pluginManifest.Spec);
    }

    [Theory]
    [FilePath("TestData/UiPluginManifest.yaml")]
    public void GivenManifestContent_ThenUiPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = (UiPluginManifest)_yamlFileReader.Read(filePath);

        Assert.Equal("v1", pluginManifest.ApiVersion);
        Assert.Equal("UI", pluginManifest.Kind);
        Assert.Equal("ScriptBee", pluginManifest.Metadata.Author);
        Assert.Equal("Description", pluginManifest.Metadata.Description);
        Assert.Equal("Plugin.dll", pluginManifest.Metadata.EntryPoint);
        Assert.Equal("Ui example", pluginManifest.Metadata.Name);
        Assert.Equal("0.0.1", pluginManifest.Metadata.Version);
        Assert.Equal("http://localhost:8080/remoteEntry.js", pluginManifest.Spec.RemoteEntry);
        Assert.Equal("uiExample", pluginManifest.Spec.RemoteName);
        Assert.Equal("./Plugin", pluginManifest.Spec.ExposedModule);
        Assert.Equal("Plugin", pluginManifest.Spec.ComponentName);
        Assert.Equal("Result", pluginManifest.Spec.UiPluginType);
    }
}
