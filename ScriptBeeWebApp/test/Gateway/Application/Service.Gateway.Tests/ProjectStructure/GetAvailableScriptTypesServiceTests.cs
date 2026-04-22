using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Plugins.Loader;
using ScriptBee.Service.Gateway.ProjectStructure;

namespace ScriptBee.Service.Gateway.Tests.ProjectStructure;

public class GetAvailableScriptTypesServiceTests
{
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();
    private readonly GetAvailableScriptTypesService _service;

    public GetAvailableScriptTypesServiceTests()
    {
        _service = new GetAvailableScriptTypesService(_pluginRepository);
    }

    [Fact]
    public void GivenNoPlugins_WhenGetAvailableScriptTypes_ThenReturnDefaults()
    {
        _pluginRepository.GetLoadedPlugins().Returns(Enumerable.Empty<Plugin>());

        var result = _service.GetAvailableScriptTypes();

        var languages = result.ToList();
        languages.Count.ShouldBe(3);
        languages.ShouldContain(new ScriptLanguage("csharp", ".cs"));
        languages.ShouldContain(new ScriptLanguage("python", ".py"));
        languages.ShouldContain(new ScriptLanguage("javascript", ".js"));
    }

    [Fact]
    public void GivenPlugins_WhenGetAvailableScriptTypes_ThenReturnDefaultsAndPlugins()
    {
        var manifest = new PluginManifest
        {
            ExtensionPoints =
            [
                new ScriptRunnerPluginExtensionPoint { Language = "lua", Extension = ".lua" },
            ],
        };
        var plugin = new Plugin("path", new PluginId("lua-plugin", new Version(1, 0)), manifest);
        _pluginRepository.GetLoadedPlugins().Returns(new List<Plugin> { plugin });

        var result = _service.GetAvailableScriptTypes();

        var languages = result.ToList();
        languages.Count.ShouldBe(4);
        languages.ShouldContain(new ScriptLanguage("csharp", ".cs"));
        languages.ShouldContain(new ScriptLanguage("python", ".py"));
        languages.ShouldContain(new ScriptLanguage("javascript", ".js"));
        languages.ShouldContain(new ScriptLanguage("lua", ".lua"));
    }

    [Fact]
    public void GivenPluginWithDefaultName_WhenGetAvailableScriptTypes_ThenDoNotDuplicate()
    {
        var manifest = new PluginManifest
        {
            ExtensionPoints =
            [
                new ScriptRunnerPluginExtensionPoint { Language = "CSharp", Extension = ".cs" },
            ],
        };
        var plugin = new Plugin("path", new PluginId("csharp-plugin", new Version(1, 0)), manifest);
        _pluginRepository.GetLoadedPlugins().Returns(new List<Plugin> { plugin });

        var result = _service.GetAvailableScriptTypes();

        result.Count().ShouldBe(3);
    }

    [Fact]
    public void GivenPluginsWithOtherExtensionPoints_WhenGetAvailableScriptTypes_ThenReturnOnlyScriptRunnerDefaultsAndPlugins()
    {
        var manifest = new PluginManifest
        {
            ExtensionPoints =
            [
                new ScriptRunnerPluginExtensionPoint { Language = "lua", Extension = ".lua" },
                new LinkerPluginExtensionPoint
                {
                    Kind = "Linker",
                    EntryPoint = "Entry",
                    Version = "1.0",
                },
            ],
        };
        var plugin = new Plugin("path", new PluginId("lua-plugin", new Version(1, 0)), manifest);
        _pluginRepository.GetLoadedPlugins().Returns(new List<Plugin> { plugin });

        var result = _service.GetAvailableScriptTypes();

        var languages = result.ToList();
        languages.Count.ShouldBe(4);
        languages.ShouldContain(new ScriptLanguage("lua", ".lua"));
        languages.ShouldNotContain(new ScriptLanguage("Linker", ""));
    }
}
