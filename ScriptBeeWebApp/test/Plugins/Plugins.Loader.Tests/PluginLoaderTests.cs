using System.Runtime.Loader;
using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Tests.Common.Plugins;

namespace ScriptBee.Plugins.Loader.Tests;

public class PluginLoaderTests
{
    private readonly IDllLoader _dllLoader = Substitute.For<IDllLoader>();
    private readonly ILogger<PluginLoader> _logger = Substitute.For<ILogger<PluginLoader>>();

    private readonly IPluginRegistrationService _pluginRegistrationService =
        Substitute.For<IPluginRegistrationService>();

    private readonly IPluginRegistry _pluginRegistry = Substitute.For<IPluginRegistry>();
    private readonly PluginLoader _pluginLoader;

    public PluginLoaderTests()
    {
        _pluginLoader = new PluginLoader(
            _logger,
            _dllLoader,
            _pluginRegistry,
            _pluginRegistrationService
        );
    }

    [Fact]
    public void GivenUnregisteredPluginKind_WhenLoad_ThenWarningIsLogged()
    {
        // Arrange
        var pluginId = new PluginId("id", new Version(1, 0, 0));
        var plugin = new TestPlugin(pluginId);
        plugin.Manifest.ExtensionPoints[0].Kind = "unregisteredKind";
        plugin.Manifest.ExtensionPoints[0].EntryPoint = "plugin.dll";

        _pluginRegistrationService
            .TryGetValue("unregisteredKind", out Arg.Any<HashSet<Type>?>())
            .Returns(false);

        // Act
        _pluginLoader.Load(plugin);

        // Assert
        _logger
            .Received(1)
            .Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(v =>
                    v != null && v.ToString()!.Contains("has no relevant Dlls to load")
                ),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()!
            );
    }

    [Fact]
    public void GivenRegisteredPluginWithNoAcceptedTypes_WhenLoad_ThenOnlyPluginManifestIsRegistered()
    {
        // Arrange
        var pluginId = new PluginId("id", new Version(1, 0, 0));
        var plugin = new TestPlugin(pluginId);
        var acceptedTypes = new HashSet<Type>();

        _pluginRegistrationService
            .TryGetValue(Arg.Any<string>(), out Arg.Any<HashSet<Type>?>())
            .Returns(x =>
            {
                x[1] = acceptedTypes;
                return true;
            });

        // Act
        _pluginLoader.Load(plugin);

        // Assert
        _pluginRegistry.Received(1).RegisterPlugin(plugin);
        _dllLoader
            .DidNotReceiveWithAnyArgs()
            .LoadDllTypes(Arg.Any<string>(), Arg.Any<ISet<Type>>());
    }

    [Fact]
    public void GivenRegisteredPluginWithAcceptedTypes_WhenLoad_ThenPluginAndTypesAreRegistered()
    {
        // Arrange
        var pluginId = new PluginId("id", new Version(1, 0, 0));
        var plugin = new TestPlugin(pluginId);
        var acceptedTypes = new HashSet<Type> { typeof(IPlugin) };
        var loadedTypes = new List<(Type, Type)> { (typeof(IPlugin), typeof(TestPlugin)) };
        var loadedPlugin = new LoadedPlugin(AssemblyLoadContext.Default, loadedTypes);

        _pluginRegistrationService
            .TryGetValue(Arg.Any<string>(), out Arg.Any<HashSet<Type>?>())
            .Returns(x =>
            {
                x[1] = acceptedTypes;
                return true;
            });

        _dllLoader.LoadDllTypes(Arg.Any<string>(), acceptedTypes).Returns(loadedPlugin);

        // Act
        _pluginLoader.Load(plugin);

        // Assert
        _pluginRegistry.Received(1).RegisterPlugin(plugin, loadedPlugin);
    }

    [Fact]
    public void GivenPluginId_WhenUnload_ThenRegistryUnRegisterIsCalled()
    {
        // Arrange
        var pluginId = new PluginId("testPlugin", new Version(1, 0, 0));

        // Act
        _pluginLoader.Unload(pluginId);

        // Assert
        _pluginRegistry.Received(1).UnRegisterPlugin(pluginId);
    }
}
