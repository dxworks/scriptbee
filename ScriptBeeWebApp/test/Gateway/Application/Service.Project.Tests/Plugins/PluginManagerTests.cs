using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Plugins;
using ScriptBee.Plugins.Loader;
using ScriptBee.Service.Project.Plugins;
using ScriptBee.Tests.Common.Plugin;

namespace ScriptBee.Service.Project.Tests.Plugins;

public class PluginManagerTests
{
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();
    private readonly IPluginLoader _pluginLoader = Substitute.For<IPluginLoader>();

    private readonly IPluginPathProvider _pluginPathProvider =
        Substitute.For<IPluginPathProvider>();

    private readonly ILogger<PluginManager> _logger = Substitute.For<ILogger<PluginManager>>();

    private readonly PluginManager _pluginManager;

    public PluginManagerTests()
    {
        _pluginManager = new PluginManager(
            _pluginReader,
            _pluginLoader,
            _pluginPathProvider,
            _logger
        );
    }

    [Fact]
    public void GivenEmptyPlugins_WhenLoadPlugins_ThenNoPluginsLoaded()
    {
        _pluginPathProvider.GetPathToPlugins().Returns("plugin/path");
        _pluginReader.ReadPlugins("plugin/path").Returns(new List<Plugin>());

        _pluginManager.LoadPlugins();

        _pluginLoader.Received(0).Load(Arg.Any<Plugin>());
    }

    [Fact]
    public void GivenAllValidPlugins_WhenLoadPlugins_ThenAllPluginsAreLoaded()
    {
        _pluginPathProvider.GetPathToPlugins().Returns("plugin/path");
        _pluginReader
            .ReadPlugins("plugin/path")
            .Returns(
                new List<Plugin>
                {
                    new TestPlugin("id", new Version(0, 0, 0, 1)),
                    new TestPlugin("id", new Version(0, 0, 0, 2)),
                    new TestPlugin("id", new Version(0, 0, 0, 3)),
                }
            );

        _pluginManager.LoadPlugins();

        _pluginLoader.Received(3).Load(Arg.Any<Plugin>());
    }

    [Fact]
    public void GivenSomeInvalidPlugins_WhenLoadPlugins_ThenAllValidPluginsAreLoaded()
    {
        var expectedException = new Exception("Test exception");

        Plugin testPlugin1 = new TestPlugin("id", new Version(0, 0, 0, 1));
        Plugin testPlugin2 = new TestPlugin("id", new Version(0, 0, 1, 1));
        Plugin testPlugin3 = new TestPlugin("id", new Version(0, 0, 2, 1));

        _pluginPathProvider.GetPathToPlugins().Returns("plugin/path");
        _pluginReader
            .ReadPlugins("plugin/path")
            .Returns(new List<Plugin> { testPlugin1, testPlugin2, testPlugin3 });
        _pluginLoader.When(x => x.Load(testPlugin2)).Throw(expectedException);

        _pluginManager.LoadPlugins();

        _logger
            .ReceivedWithAnyArgs()
            .LogError(expectedException, "Failed to load plugin {Plugin}", testPlugin2);
    }
}
