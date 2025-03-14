using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Service.Plugins.Tests;

public class PluginRepositoryTests
{
    private readonly PluginRepository _repository = new();

    [Fact]
    public void RegisterPlugin_AddsPluginToRepository()
    {
        var plugin = CreateTestPlugin("testId", new Version(1, 0, 0));

        _repository.RegisterPlugin(plugin);

        Assert.Single(_repository.GetLoadedPluginsManifests());
        Assert.Equal("testId", _repository.GetLoadedPluginsManifests().First().Name);
    }

    [Fact]
    public void RegisterPlugin_ReplacesOlderVersion()
    {
        var plugin1 = CreateTestPlugin("testId", new Version(1, 0, 0));
        var plugin2 = CreateTestPlugin("testId", new Version(2, 0, 0));

        _repository.RegisterPlugin(plugin1);
        _repository.RegisterPlugin(plugin2);

        Assert.Single(_repository.GetLoadedPluginsManifests());
        Assert.Equal(new Version(2, 0, 0), _repository.GetInstalledPluginVersion("testId"));
    }

    [Fact]
    public void RegisterPlugin_DoesNotReplaceNewerVersion()
    {
        var plugin1 = CreateTestPlugin("testId", new Version(2, 0, 0));
        var plugin2 = CreateTestPlugin("testId", new Version(1, 0, 0));

        _repository.RegisterPlugin(plugin1);
        _repository.RegisterPlugin(plugin2);

        Assert.Single(_repository.GetLoadedPluginsManifests());
        Assert.Equal(new Version(2, 0, 0), _repository.GetInstalledPluginVersion("testId"));
    }

    [Fact]
    public void UnRegisterPlugin_RemovesPluginFromRepository()
    {
        var plugin = CreateTestPlugin("testId", new Version(1, 0, 0));
        _repository.RegisterPlugin(plugin);

        _repository.UnRegisterPlugin("testId", "1.0.0");

        Assert.Empty(_repository.GetLoadedPluginsManifests());
    }

    [Fact]
    public void RegisterPlugin_WithInterfaceAndConcrete_AddsServiceDescriptor()
    {
        var plugin = CreateTestPlugin("testId", new Version(1, 0, 0));

        _repository.RegisterPlugin(plugin, typeof(ITestPlugin), typeof(TestPlugin));

        var services = _repository.GetPlugins<ITestPlugin>();
        Assert.Single(services);
    }

    [Fact]
    public void GetPlugin_ReturnsPluginMatchingFilter()
    {
        var plugin1 = CreateTestPlugin("testId1", new Version(1, 0, 0));
        var plugin2 = CreateTestPlugin("testId2", new Version(1, 0, 0));
        _repository.RegisterPlugin(plugin1, typeof(ITestPlugin), typeof(TestPlugin));
        _repository.RegisterPlugin(plugin2, typeof(ITestPlugin), typeof(TestPlugin2));

        var result = _repository.GetPlugin<ITestPlugin>(p => p.GetPluginId() == "testId2");

        Assert.NotNull(result);
        Assert.Equal("testId2", result.GetPluginId());
    }

    [Fact]
    public void GetPlugins_ReturnsAllPluginsOfGivenType()
    {
        var plugin1 = CreateTestPlugin("testId1", new Version(1, 0, 0));
        var plugin2 = CreateTestPlugin("testId2", new Version(1, 0, 0));
        _repository.RegisterPlugin(plugin1, typeof(ITestPlugin), typeof(TestPlugin));
        _repository.RegisterPlugin(plugin2, typeof(ITestPlugin), typeof(TestPlugin2));

        var result = _repository.GetPlugins<ITestPlugin>();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetLoadedPluginsManifests_ReturnsManifestsOfLoadedPlugins()
    {
        var plugin1 = CreateTestPlugin("testId1", new Version(1, 0, 0));
        var plugin2 = CreateTestPlugin("testId2", new Version(1, 0, 0));
        _repository.RegisterPlugin(plugin1);
        _repository.RegisterPlugin(plugin2);

        var manifests = _repository.GetLoadedPluginsManifests().ToList();

        Assert.Equal(2, manifests.Count);
        Assert.Contains(manifests, m => m.Name == "testId1");
        Assert.Contains(manifests, m => m.Name == "testId2");
    }

    [Fact]
    public void GetLoadedPlugins_WithKind_ReturnsPluginsWithMatchingKind()
    {
        var plugin1 = CreateTestPlugin(
            "testId1",
            new Version(1, 0, 0),
            new TestExtensionPoint { Kind = "testKind" }
        );
        var plugin2 = CreateTestPlugin(
            "testId2",
            new Version(1, 0, 0),
            new TestExtensionPoint { Kind = "anotherKind" }
        );
        _repository.RegisterPlugin(plugin1);
        _repository.RegisterPlugin(plugin2);

        var plugins = _repository.GetLoadedPlugins("testKind").ToList();

        Assert.Single(plugins);
        Assert.Equal("testId1", plugins.First().Id);
    }

    [Fact]
    public void GetLoadedPluginExtensionPoints_ReturnsExtensionPointsOfGivenType()
    {
        var plugin1 = CreateTestPlugin(
            "testId1",
            new Version(1, 0, 0),
            new TestExtensionPoint { Kind = "testKind" }
        );
        var plugin2 = CreateTestPlugin(
            "testId2",
            new Version(1, 0, 0),
            new AnotherTestExtensionPoint { Kind = "anotherKind" }
        );
        _repository.RegisterPlugin(plugin1);
        _repository.RegisterPlugin(plugin2);

        var extensionPoints = _repository
            .GetLoadedPluginExtensionPoints<TestExtensionPoint>()
            .ToList();

        Assert.Single(extensionPoints);
        Assert.Equal("testKind", extensionPoints.First().Kind);
    }

    [Fact]
    public void GetInstalledPluginVersion_ReturnsInstalledPluginVersion()
    {
        var plugin = CreateTestPlugin("testId", new Version(1, 0, 0));
        _repository.RegisterPlugin(plugin);

        var version = _repository.GetInstalledPluginVersion("testId");
        Assert.Equal(new Version(1, 0, 0), version);
    }

    [Fact]
    public void GetInstalledPluginVersion_ReturnsNullIfPluginNotInstalled()
    {
        var version = _repository.GetInstalledPluginVersion("nonExistentId");

        Assert.Null(version);
    }

    private static Plugin CreateTestPlugin(
        string id,
        Version version,
        params PluginExtensionPoint[] extensionPoints
    )
    {
        return new Plugin(
            "test/path",
            id,
            version,
            new PluginManifest { Name = id, ExtensionPoints = extensionPoints.ToList() }
        );
    }
}

file class TestExtensionPoint : PluginExtensionPoint;

file class AnotherTestExtensionPoint : PluginExtensionPoint;

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
