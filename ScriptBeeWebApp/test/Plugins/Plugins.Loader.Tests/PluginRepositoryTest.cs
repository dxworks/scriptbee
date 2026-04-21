using System.Runtime.Loader;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Plugins.Loader.Tests;

public class PluginRepositoryTests
{
    private readonly PluginRepository _repository = new();

    [Fact]
    public void GivenValidPlugin_WhenRegister_ThenServiceIsAdded()
    {
        // Arrange
        var plugin = CreateTestPlugin("testId", new Version(1, 0, 0));
        var loadedPlugin = new LoadedPlugin(
            new AssemblyLoadContext(null, isCollectible: true),
            new List<(Type, Type)> { (typeof(ITestPlugin), typeof(TestPlugin)) }
        );

        // Act
        _repository.RegisterPlugin(plugin, loadedPlugin);

        // Assert
        var services = _repository.GetPlugins<ITestPlugin>();
        Assert.Single(services);
    }

    [Fact]
    public void GivenMultiplePlugins_WhenGetPluginWithFilter_ThenCorrectPluginIsReturned()
    {
        // Arrange
        var plugin1 = CreateTestPlugin("testId1", new Version(1, 0, 0));
        var plugin2 = CreateTestPlugin("testId2", new Version(1, 0, 0));
        var loadedPlugin1 = new LoadedPlugin(
            new AssemblyLoadContext(null, isCollectible: true),
            new List<(Type, Type)> { (typeof(ITestPlugin), typeof(TestPlugin)) }
        );
        var loadedPlugin2 = new LoadedPlugin(
            new AssemblyLoadContext(null, isCollectible: true),
            new List<(Type, Type)> { (typeof(ITestPlugin), typeof(TestPlugin2)) }
        );
        _repository.RegisterPlugin(plugin1, loadedPlugin1);
        _repository.RegisterPlugin(plugin2, loadedPlugin2);

        // Act
        var result = _repository.GetPlugin<ITestPlugin>(p => p.GetPluginId() == "testId2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testId2", result.GetPluginId());
    }

    [Fact]
    public void GivenMultiplePlugins_WhenGetPlugins_ThenAllAreReturned()
    {
        // Arrange
        var plugin1 = CreateTestPlugin("testId1", new Version(1, 0, 0));
        var plugin2 = CreateTestPlugin("testId2", new Version(1, 0, 0));
        var loadedPlugin1 = new LoadedPlugin(
            new AssemblyLoadContext(null, isCollectible: true),
            new List<(Type, Type)> { (typeof(ITestPlugin), typeof(TestPlugin)) }
        );
        var loadedPlugin2 = new LoadedPlugin(
            new AssemblyLoadContext(null, isCollectible: true),
            new List<(Type, Type)> { (typeof(ITestPlugin), typeof(TestPlugin2)) }
        );
        _repository.RegisterPlugin(plugin1, loadedPlugin1);
        _repository.RegisterPlugin(plugin2, loadedPlugin2);

        // Act
        var result = _repository.GetPlugins<ITestPlugin>();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GivenRegisteredPlugin_WhenUnRegister_ThenPluginAndServicesAreRemoved()
    {
        // Arrange
        var pluginId = new PluginId("testId", new Version(1, 0, 0));
        var plugin = CreateTestPlugin(pluginId.Name, pluginId.Version);
        var loadedPlugin = new LoadedPlugin(
            new AssemblyLoadContext(null, isCollectible: true),
            new List<(Type, Type)> { (typeof(ITestPlugin), typeof(TestPlugin)) }
        );
        _repository.RegisterPlugin(plugin, loadedPlugin);

        // Act
        _repository.UnRegisterPlugin(pluginId);

        // Assert
        var plugins = _repository.GetLoadedPlugins();
        var services = _repository.GetPlugins<ITestPlugin>();
        Assert.Empty(plugins);
        Assert.Empty(services);
    }

    private static Plugin CreateTestPlugin(
        string id,
        Version version,
        params PluginExtensionPoint[] extensionPoints
    )
    {
        return new Plugin(
            "test/path",
            new PluginId(id, version),
            new PluginManifest { Name = id, ExtensionPoints = extensionPoints.ToList() }
        );
    }
}

file interface ITestPlugin : IPlugin
{
    string GetPluginId();
}

file class TestPlugin : ITestPlugin
{
    public string GetPluginId()
    {
        return "testId1";
    }
}

file class TestPlugin2 : ITestPlugin
{
    public string GetPluginId()
    {
        return "testId2";
    }
}
