﻿using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Persistence.File.Plugin;
using ScriptBee.Tests.Common;

namespace ScriptBee.Persistence.File.Tests.Plugin.Manifest;

public class PluginManifestYamlParsingTests
{
    private readonly PluginManifestYamlFileReader _yamlFileReader = new(
        new PluginDiscriminatorHolder()
    );

    [Theory]
    [FilePath("TestData/ScriptGeneratorPluginManifest.yaml")]
    public void GivenManifestContent_ThenScriptGeneratorPluginIsConstructed(string filePath)
    {
        var pluginManifest = _yamlFileReader.Read(filePath);

        Assert.Equal("1.0.0", pluginManifest.ApiVersion);
        Assert.Equal("ScriptGenerator example", pluginManifest.Name);
        Assert.Equal("ScriptBee", pluginManifest.Author);
        Assert.Equal("Description", pluginManifest.Description);

        var extensionPoint = (ScriptGeneratorPluginExtensionPoint)
            pluginManifest.ExtensionPoints.Single();

        Assert.Equal("ScriptGenerator", extensionPoint.Kind);
        Assert.Equal("Plugin.dll", extensionPoint.EntryPoint);
        Assert.Equal("0.0.1", extensionPoint.Version);
        Assert.Equal("csharp", extensionPoint.Language);
        Assert.Equal(".cs", extensionPoint.Extension);
    }

    [Theory]
    [FilePath("TestData/ScriptRunnerPluginManifest.yaml")]
    public void GivenManifestContent_ThenScriptRunnerPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = _yamlFileReader.Read(filePath);

        Assert.Equal("1.0.0", pluginManifest.ApiVersion);
        Assert.Equal("ScriptRunner example", pluginManifest.Name);
        Assert.Equal("ScriptBee", pluginManifest.Author);
        Assert.Equal("Description", pluginManifest.Description);

        var extensionPoint = (ScriptRunnerPluginExtensionPoint)
            pluginManifest.ExtensionPoints.Single();

        Assert.Equal("ScriptRunner", extensionPoint.Kind);
        Assert.Equal("Plugin.dll", extensionPoint.EntryPoint);
        Assert.Equal("0.0.1", extensionPoint.Version);
        Assert.Equal("csharp", extensionPoint.Language);
    }

    [Theory]
    [FilePath("TestData/HelperFunctionsPluginManifest.yaml")]
    public void GivenManifestContent_ThenHelperFunctionsPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = _yamlFileReader.Read(filePath);

        Assert.Equal("1.0.0", pluginManifest.ApiVersion);
        Assert.Equal("HelperFunctions example", pluginManifest.Name);
        Assert.Equal("ScriptBee", pluginManifest.Author);
        Assert.Equal("Description", pluginManifest.Description);

        var extensionPoint = (HelperFunctionsPluginExtensionPoint)
            pluginManifest.ExtensionPoints.Single();

        Assert.Equal("HelperFunctions", extensionPoint.Kind);
        Assert.Equal("Plugin.dll", extensionPoint.EntryPoint);
        Assert.Equal("0.0.1", extensionPoint.Version);
    }

    [Theory]
    [FilePath("TestData/LoaderPluginManifest.yaml")]
    public void GivenManifestContent_ThenLoaderPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = _yamlFileReader.Read(filePath);

        Assert.Equal("1.0.0", pluginManifest.ApiVersion);
        Assert.Equal("Loader example", pluginManifest.Name);
        Assert.Equal("ScriptBee", pluginManifest.Author);
        Assert.Equal("Description", pluginManifest.Description);

        var extensionPoint = (LoaderPluginExtensionPoint)pluginManifest.ExtensionPoints.Single();

        Assert.Equal("Loader", extensionPoint.Kind);
        Assert.Equal("Plugin.dll", extensionPoint.EntryPoint);
        Assert.Equal("0.0.1", extensionPoint.Version);
    }

    [Theory]
    [FilePath("TestData/LinkerPluginManifest.yaml")]
    public void GivenManifestContent_ThenLinkerPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = _yamlFileReader.Read(filePath);

        Assert.Equal("1.0.0", pluginManifest.ApiVersion);
        Assert.Equal("Linker example", pluginManifest.Name);
        Assert.Equal("ScriptBee", pluginManifest.Author);
        Assert.Equal("Description", pluginManifest.Description);

        var extensionPoint = (LinkerPluginExtensionPoint)pluginManifest.ExtensionPoints.Single();

        Assert.Equal("Linker", extensionPoint.Kind);
        Assert.Equal("Plugin.dll", extensionPoint.EntryPoint);
        Assert.Equal("0.0.1", extensionPoint.Version);
    }

    [Theory]
    [FilePath("TestData/UiPluginManifest.yaml")]
    public void GivenManifestContent_ThenUiPluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = _yamlFileReader.Read(filePath);
        Assert.Equal("1.0.0", pluginManifest.ApiVersion);
        Assert.Equal("Ui example", pluginManifest.Name);
        Assert.Equal("ScriptBee", pluginManifest.Author);
        Assert.Equal("Description", pluginManifest.Description);

        var extensionPoint = (UiPluginExtensionPoint)pluginManifest.ExtensionPoints.Single();

        Assert.Equal("UI", extensionPoint.Kind);
        Assert.Equal("Plugin.dll", extensionPoint.EntryPoint);
        Assert.Equal("0.0.1", extensionPoint.Version);
        Assert.Equal(8080, extensionPoint.Port);
        Assert.Equal("http://localhost:8080/remoteEntry.js", extensionPoint.RemoteEntry);
        Assert.Equal("./Plugin", extensionPoint.ExposedModule);
        Assert.Equal("Plugin", extensionPoint.ComponentName);
        Assert.Equal("Result", extensionPoint.UiPluginType);
    }

    [Theory]
    [FilePath("TestData/PluginBundlePluginManifest.yaml")]
    public void GivenManifestContent_ThenPluginBundlePluginManifestIsConstructed(string filePath)
    {
        var pluginManifest = _yamlFileReader.Read(filePath);

        Assert.Equal("1.0.0", pluginManifest.ApiVersion);
        Assert.Equal("Plugin example", pluginManifest.Name);
        Assert.Equal("ScriptBee", pluginManifest.Author);
        Assert.Equal("Description", pluginManifest.Description);

        var extensionPoint = (PluginBundleExtensionPoint)pluginManifest.ExtensionPoints.Single();

        Assert.Equal("Plugin", extensionPoint.Kind);
        Assert.Equal("plugin", extensionPoint.EntryPoint);
        Assert.Equal("0.0.1", extensionPoint.Version);
    }
}
