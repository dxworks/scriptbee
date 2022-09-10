using Moq;
using ScriptBee.Plugin;
using ScriptBee.Services;
using ScriptBee.Tests.Plugin.Internals;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginLoaderFactoryTests
{
    private readonly PluginLoaderFactory _pluginLoaderFactory;

    public PluginLoaderFactoryTests()
    {
        var loadersHolderMock = new Mock<ILoadersHolder>();
        var linkersHolderMock = new Mock<ILinkersHolder>();
        var scriptGeneratorStrategyHolderMock = new Mock<IScriptGeneratorStrategyHolder>();

        _pluginLoaderFactory = new PluginLoaderFactory(loadersHolderMock.Object, linkersHolderMock.Object,
            scriptGeneratorStrategyHolderMock.Object);
    }

    [Fact]
    public void GivenModelLoaderPlugin_WhenGetPluginLoader_ThenLoaderPluginLoaderIsReturned()
    {
        var pluginLoader = _pluginLoaderFactory.GetPluginLoader(new TestLoaderPlugin().GetType());

        Assert.IsType<LoaderPluginLoader>(pluginLoader);
    }

    [Fact]
    public void GivenModelLinkerPlugin_WhenGetPluginLoader_ThenLinkerPluginLoaderIsReturned()
    {
        var pluginLoader = _pluginLoaderFactory.GetPluginLoader(new TestLinkerPlugin().GetType());

        Assert.IsType<LinkerPluginLoader>(pluginLoader);
    }

    [Fact]
    public void GivenScriptGeneratorPlugin_WhenGetPluginLoader_ThenScriptGeneratorPluginLoaderIsReturned()
    {
        var pluginLoader = _pluginLoaderFactory.GetPluginLoader(new TestScriptGeneratorPlugin().GetType());

        Assert.IsType<ScriptGeneratorPluginLoader>(pluginLoader);
    }

    [Fact]
    public void GivenInvalidPlugin_WhenGetPluginLoader_ThenNullIsReturned()
    {
        var pluginLoader = _pluginLoaderFactory.GetPluginLoader(typeof(string));

        Assert.Null(pluginLoader);
    }
}
