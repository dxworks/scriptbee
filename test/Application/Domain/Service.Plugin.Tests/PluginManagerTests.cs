using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Service.Plugin.Tests.Internals;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Service.Plugin.Tests;

public class PluginManagerTests
{
    private readonly IPluginReader _pluginReader = Substitute.For<IPluginReader>();
    private readonly IPluginLoader _pluginLoader = Substitute.For<IPluginLoader>();
    private readonly ILogger<PluginManager> _logger = Substitute.For<ILogger<PluginManager>>();

    private readonly PluginManager _pluginManager;

    public PluginManagerTests()
    {
        _pluginManager = new PluginManager(_pluginReader, _pluginLoader, _logger);
    }

    [Fact]
    public void GivenEmptyPlugins_WhenLoadPlugins_ThenNoPluginsLoaded()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(new List<Domain.Model.Plugin.Plugin>());

        _pluginManager.LoadPlugins();

        _pluginLoader.Received(0).Load(Arg.Any<Domain.Model.Plugin.Plugin>());
    }

    [Fact]
    public void GivenAllValidPlugins_WhenLoadPlugins_ThenAllPluginsAreLoaded()
    {
        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(
                new List<Domain.Model.Plugin.Plugin>
                {
                    new TestPlugin("id", new Version(0, 0, 0, 1)),
                    new TestPlugin("id", new Version(0, 0, 0, 2)),
                    new TestPlugin("id", new Version(0, 0, 0, 3)),
                }
            );

        _pluginManager.LoadPlugins();

        _pluginLoader.Received(3).Load(Arg.Any<Domain.Model.Plugin.Plugin>());
    }

    [Fact]
    public void GivenSomeInvalidPlugins_WhenLoadPlugins_ThenAllValidPluginsAreLoaded()
    {
        var expectedException = new Exception("Test exception");

        Domain.Model.Plugin.Plugin testPlugin1 = new TestPlugin("id", new Version(0, 0, 0, 1));
        Domain.Model.Plugin.Plugin testPlugin2 = new TestPlugin("id", new Version(0, 0, 1, 1));
        Domain.Model.Plugin.Plugin testPlugin3 = new TestPlugin("id", new Version(0, 0, 2, 1));

        _pluginReader
            .ReadPlugins(ConfigFolders.PathToPlugins)
            .Returns(
                new List<Domain.Model.Plugin.Plugin> { testPlugin1, testPlugin2, testPlugin3 }
            );
        _pluginLoader.When(x => x.Load(testPlugin2)).Throw(expectedException);

        _pluginManager.LoadPlugins();

        _logger
            .Received(1)
            .LogError(expectedException, "Failed to load plugin {Plugin}", testPlugin2);
    }
}
