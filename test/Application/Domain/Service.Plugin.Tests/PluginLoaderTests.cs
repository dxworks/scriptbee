using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Plugin.Tests.Internals;

namespace ScriptBee.Service.Plugin.Tests;

public class PluginLoaderTests
{
    private readonly IDllLoader _dllLoader = Substitute.For<IDllLoader>();
    private readonly ILogger<PluginLoader> _logger = Substitute.For<ILogger<PluginLoader>>();

    private readonly IPluginRegistrationService _pluginRegistrationService =
        Substitute.For<IPluginRegistrationService>();

    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();

    private readonly PluginLoader _pluginLoader;

    public PluginLoaderTests()
    {
        _pluginLoader = new PluginLoader(
            _logger,
            _dllLoader,
            _pluginRepository,
            _pluginRegistrationService
        );
    }

    [Fact]
    public void GivenUnregisteredPlugin_WhenLoad_ThenMessageIsLogged()
    {
        HashSet<Type>? nullTypes = null;
        var plugin = new TestPlugin("id", new Version(0, 0, 0, 1));
        plugin.Manifest.ExtensionPoints[0].Kind = "kind";
        plugin.Manifest.ExtensionPoints[0].EntryPoint = "entryPoint";
        _pluginRegistrationService
            .TryGetValue(Arg.Any<string>(), out Arg.Any<HashSet<Type>?>())
            .Returns(x =>
            {
                x[1] = nullTypes;
                return false;
            });

        _pluginLoader.Load(plugin);

        _logger
            .Received(1)
            .ReceivedWithAnyArgs()
            .LogWarning(
                "Plugin kind '{PluginKind}' from '{EntryPoint}' has no relevant Dlls to load",
                "kind",
                "entryPoint"
            );
    }

    [Fact]
    public void GivenRegisteredPluginWithNoAcceptedTypes_WhenLoad_ThenPluginManifestIsLoaded()
    {
        var acceptedTypes = new HashSet<Type>();
        var plugin = new TestPlugin("id", new Version(0, 0, 0, 1));
        var entryPoint = Path.Combine(
            plugin.FolderPath,
            plugin.Manifest.ExtensionPoints[0].EntryPoint
        );
        _pluginRegistrationService
            .TryGetValue(Arg.Any<string>(), out Arg.Any<HashSet<Type>?>())
            .Returns(x =>
            {
                x[1] = acceptedTypes;
                return true;
            });
        _dllLoader
            .LoadDllTypes(entryPoint, acceptedTypes)
            .Returns(new List<(Type @interface, Type concrete)>());

        _pluginLoader.Load(plugin);

        _pluginRepository.Received(1).ReceivedWithAnyArgs().RegisterPlugin(plugin);
    }

    [Fact]
    public void GivenRegisteredPluginWithMultipleAcceptedTypes_WhenLoad_ThenPluginManifestIsLoaded()
    {
        var acceptedTypes = new HashSet<Type> { typeof(string), typeof(object) };
        var plugin = new TestPlugin("id", new Version(0, 0, 0, 1));
        var entryPoint = Path.Combine(
            plugin.FolderPath,
            plugin.Manifest.ExtensionPoints[0].EntryPoint
        );
        _pluginRegistrationService
            .TryGetValue(Arg.Any<string>(), out Arg.Any<HashSet<Type>?>())
            .Returns(x =>
            {
                x[1] = acceptedTypes;
                return true;
            });
        _dllLoader
            .LoadDllTypes(entryPoint, acceptedTypes)
            .Returns(
                new List<(Type @interface, Type concrete)>
                {
                    (typeof(string), typeof(string)),
                    (typeof(object), typeof(object)),
                }
            );

        _pluginLoader.Load(plugin);

        _pluginRepository
            .Received(1)
            .ReceivedWithAnyArgs()
            .RegisterPlugin(plugin, typeof(object), typeof(object));
    }
}
