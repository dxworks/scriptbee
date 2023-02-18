using System.Collections.Generic;
using System.Linq;
using Moq;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Services;

public class GenerateScriptServiceTests
{
    private readonly Mock<ILoadersService> _loadersServiceMock;
    private readonly Mock<IPluginRepository> _pluginRepository;

    private readonly GenerateScriptService _generateScriptService;

    public GenerateScriptServiceTests()
    {
        _loadersServiceMock = new Mock<ILoadersService>();
        _pluginRepository = new Mock<IPluginRepository>();

        _generateScriptService = new GenerateScriptService(_loadersServiceMock.Object, _pluginRepository.Object);
    }

    [Fact]
    public void GivenSupportedLanguages_WhenGetSupportedLanguages_ThenSupportedLanguagesAreReturned()
    {
        _pluginRepository.Setup(x => x.GetLoadedPluginExtensionPoints<ScriptGeneratorPluginExtensionPoint>())
            .Returns(new List<ScriptGeneratorPluginExtensionPoint>
            {
                new()
                {
                    Language = "language1",
                    Extension = "extension1",
                },
                new()
                {
                    Language = "language2",
                    Extension = "extension2",
                }
            });

        var result = _generateScriptService.GetSupportedLanguages().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("language1", result[0].Name);
        Assert.Equal("extension1", result[0].Extension);
        Assert.Equal("language2", result[1].Name);
        Assert.Equal("extension2", result[1].Extension);
    }
}
