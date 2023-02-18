using Microsoft.Extensions.DependencyInjection;
using Moq;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Tests.Contract.ProviderStates;

public class ScriptsEndpointStates : IProviderStateDefinition
{
    private readonly Mock<IGenerateScriptService> _generateScriptServiceMock = new();
    private readonly Mock<IProjectManager> _projectManagerMock = new();


    public void RegisterMocks(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IGenerateScriptService>(_ => _generateScriptServiceMock.Object);
        serviceCollection.AddSingleton<IProjectManager>(_ => _projectManagerMock.Object);
    }

    [ProviderState("existing script languages")]
    public void ExistingScriptLanguages()
    {
        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("Javascript", "js"),
            });
    }

    [ProviderState("an error while getting script languages")]
    public void ErrorWhileGettingScriptLanguages()
    {
        _generateScriptServiceMock.Setup(x => x.GetSupportedLanguages())
            .Throws(new Exception());
    }
}
