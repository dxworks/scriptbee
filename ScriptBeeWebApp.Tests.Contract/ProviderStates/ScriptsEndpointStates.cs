using Microsoft.Extensions.DependencyInjection;
using Moq;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Data;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Tests.Contract.ProviderStates;

public class ScriptsEndpointStates : IProviderStateDefinition
{
    private readonly Mock<IGenerateScriptService> _generateScriptServiceMock = new();
    private readonly Mock<IProjectManager> _projectManagerMock = new();
    private readonly Mock<IScriptsService> _scriptsServiceMock = new();


    public void RegisterMocks(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IScriptsService>(_ => _scriptsServiceMock.Object);
        // TODO: remove the need for this
        serviceCollection.AddSingleton<IGenerateScriptService>(_ => _generateScriptServiceMock.Object);
        serviceCollection.AddSingleton<IProjectManager>(_ => _projectManagerMock.Object);
    }

    [ProviderState("existing script languages")]
    public void ExistingScriptLanguages()
    {
        _scriptsServiceMock.Setup(x => x.GetSupportedLanguages())
            .Returns(new List<ScriptLanguage>
            {
                new("Javascript", "js"),
            });
    }

    [ProviderState("an error while getting script languages")]
    public void ErrorWhileGettingScriptLanguages()
    {
        _scriptsServiceMock.Setup(x => x.GetSupportedLanguages())
            .Throws(new TestException());
    }

    [ProviderState("create script with empty project id")]
    public void CreateScriptWithEmptyProjectId()
    {
        // handled by the validator   
    }

    [ProviderState("create script with empty script path")]
    public void CreateScriptWithEmptyScriptPath()
    {
        // handled by the validator   
    }

    [ProviderState("create script with empty script type")]
    public void CreateScriptWithEmptyScriptType()
    {
        // handled by the validator   
    }

    [ProviderState("create script with invalid script type")]
    public void CreateScriptWithInvalidScriptType()
    {
        _scriptsServiceMock.Setup(x => x.CreateScriptAsync(It.IsAny<CreateScript>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InvalidScriptType("invalid-type"));
    }

    [ProviderState("create script with project id for project that does not exist")]
    public void CreateScriptWithProjectIdForProjectThatDoesNotExist()
    {
        _scriptsServiceMock.Setup(x => x.CreateScriptAsync(It.IsAny<CreateScript>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectMissing("project-id"));
    }

    [ProviderState("create script with script path that already exists")]
    public void CreateScriptWithScriptPathThatAlreadyExists()
    {
        _scriptsServiceMock.Setup(x => x.CreateScriptAsync(It.IsAny<CreateScript>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptConflict());
    }

    [ProviderState("error while creating script")]
    public void ErrorWhileCreatingScript()
    {
        _scriptsServiceMock.Setup(x => x.CreateScriptAsync(It.IsAny<CreateScript>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TestException());
    }

    [ProviderState("valid data to create a script")]
    public void ValidDataToCreateAScript()
    {
        _scriptsServiceMock.Setup(x => x.CreateScriptAsync(It.IsAny<CreateScript>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateScriptResponse("id", "projectId", "filePath", "filePath", "srcPath",
                "scriptType", new List<ScriptParameterResponse>
                {
                    new("parameter name", "string", "parameter value")
                }));
    }
}
