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
        Assert.Equal("csharp",pluginManifest.Spec.Language);
        Assert.Equal(".cs",pluginManifest.Spec.Extension);
    }

    [Theory]
    [FilePath("TestData/ScriptRunnerPluginManifest.yaml")]
    public void GivenManifestContent_ThenScriptRunnerPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = (ScriptRunnerPluginManifest)_yamlFileReader.Read(filePath);

        Assert.Equal("v1", pluginManifest.ApiVersion);
        Assert.Equal("ScriptRunner", pluginManifest.Kind);
        Assert.Equal("ScriptBee", pluginManifest.Metadata.Author);
        Assert.Equal("Description", pluginManifest.Metadata.Description);
        Assert.Equal("Plugin.dll", pluginManifest.Metadata.EntryPoint);
        Assert.Equal("ScriptRunner example", pluginManifest.Metadata.Name);
        Assert.Equal("0.0.1", pluginManifest.Metadata.Version);
        Assert.Equal("csharp",pluginManifest.Spec.Language);
    }
    
    [Theory]
    [FilePath("TestData/HelperFunctionsPluginManifest.yaml")]
    public void GivenManifestContent_ThenHelperFunctionsPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = (HelperFunctionsPluginManifest)_yamlFileReader.Read(filePath);

        Assert.Equal("v1", pluginManifest.ApiVersion);
        Assert.Equal("HelperFunctions", pluginManifest.Kind);
        Assert.Equal("ScriptBee", pluginManifest.Metadata.Author);
        Assert.Equal("Description", pluginManifest.Metadata.Description);
        Assert.Equal("Plugin.dll", pluginManifest.Metadata.EntryPoint);
        Assert.Equal("HelperFunctions example", pluginManifest.Metadata.Name);
        Assert.Equal("0.0.1", pluginManifest.Metadata.Version);
    }
    
    [Theory]
    [FilePath("TestData/LoaderPluginManifest.yaml")]
    public void GivenManifestContent_ThenLoaderPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = (LoaderPluginManifest)_yamlFileReader.Read(filePath);

        Assert.Equal("v1", pluginManifest.ApiVersion);
        Assert.Equal("Loader", pluginManifest.Kind);
        Assert.Equal("ScriptBee", pluginManifest.Metadata.Author);
        Assert.Equal("Description", pluginManifest.Metadata.Description);
        Assert.Equal("Plugin.dll", pluginManifest.Metadata.EntryPoint);
        Assert.Equal("Loader example", pluginManifest.Metadata.Name);
        Assert.Equal("0.0.1", pluginManifest.Metadata.Version);
    }
    
    [Theory]
    [FilePath("TestData/LinkerPluginManifest.yaml")]
    public void GivenManifestContent_ThenLinkerPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = (LinkerPluginManifest)_yamlFileReader.Read(filePath);

        Assert.Equal("v1", pluginManifest.ApiVersion);
        Assert.Equal("Linker", pluginManifest.Kind);
        Assert.Equal("ScriptBee", pluginManifest.Metadata.Author);
        Assert.Equal("Description", pluginManifest.Metadata.Description);
        Assert.Equal("Plugin.dll", pluginManifest.Metadata.EntryPoint);
        Assert.Equal("Linker example", pluginManifest.Metadata.Name);
        Assert.Equal("0.0.1", pluginManifest.Metadata.Version);
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
        Assert.Equal(8080, pluginManifest.Spec.Port);
        Assert.Equal("http://localhost:8080/remoteEntry.js", pluginManifest.Spec.RemoteEntry);
        Assert.Equal("./Plugin", pluginManifest.Spec.ExposedModule);
        Assert.Equal("Plugin", pluginManifest.Spec.ComponentName);
        Assert.Equal("Result", pluginManifest.Spec.UiPluginType);
    }
}
