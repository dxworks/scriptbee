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

    [ProviderState("valid project id while getting scripts")]
    public void ValidProjectIdWhileGettingScripts()
    {
        _scriptsServiceMock.Setup(x =>
                x.GetScriptsStructureAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ScriptFileStructureNode>
            {
                new()
                {
                    Name = "name",
                    Path = "path",
                    AbsolutePath = "absolutePath",
                    IsDirectory = true,
                    ScriptData = new ScriptDataResponse("id", "project-id", "name", "path", "absolutePath", "language",
                        new List<ScriptParameterResponse>
                        {
                            new("name", "string", "value")
                        }),
                    Children = new List<ScriptFileStructureNode>
                    {
                        new()
                        {
                            Name = "name",
                            Path = "path",
                            AbsolutePath = "absolutePath",
                            IsDirectory = true,
                            ScriptData = new ScriptDataResponse("id", "project-id", "name", "path", "absolutePath",
                                "language",
                                new List<ScriptParameterResponse>
                                {
                                    new("name", "string", "value")
                                }),
                        }
                    }
                }
            });
    }

    [ProviderState("empty project id for get scripts")]
    public void EmptyProjectIdForGetScripts()
    {
        // handled by the validator 
    }

    [ProviderState("project id for project that does not exist for get scripts")]
    public void MissingProjectIdForGetScripts()
    {
        _scriptsServiceMock.Setup(x =>
                x.GetScriptsStructureAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectMissing("project-id"));
    }

    [ProviderState("error while getting scripts")]
    public void ErrorWhileGettingScripts()
    {
        _scriptsServiceMock.Setup(x => x.GetScriptsStructureAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new TestException());
    }

    [ProviderState("empty project id for get script by id")]
    public void EmptyProjectIdForGetScriptById()
    {
        // handled by the validator   
    }

    [ProviderState("project id for project that does not exist for get script by id")]
    public void ProjectIdForProjectThatDoesNotExistForGetScriptById()
    {
        _scriptsServiceMock.Setup(x =>
                x.GetScriptByFilePathAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectMissing("project-id"));
    }

    [ProviderState("error while getting script by id")]
    public void ErrorWhileGettingScriptById()
    {
        _scriptsServiceMock.Setup(x =>
                x.GetScriptByFilePathAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new TestException());
    }


    [ProviderState("valid script id and project id for get script content")]
    public void ValidScriptIdAndProjectIdForGetScriptContent()
    {
        _scriptsServiceMock.Setup(x =>
                x.GetScriptContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("script content");
    }

    [ProviderState("empty project id for get script content")]
    public void EmptyProjectIdForGetScriptContent()
    {
        // handled by the validator
    }

    [ProviderState("project id for project that does not exist for get script content")]
    public void ProjectIdForProjectThatDoesNotExistForGetScriptContent()
    {
        _scriptsServiceMock.Setup(x =>
                x.GetScriptContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectMissing("project-id"));
    }

    [ProviderState("script id for project that does not exist for get script content")]
    public void ScriptIdForProjectThatDoesNotExistForGetScriptContent()
    {
        _scriptsServiceMock.Setup(x =>
                x.GetScriptContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptMissing("script-id"));
    }

    [ProviderState("error while getting script content")]
    public void ErrorWhileGettingScriptContent()
    {
        _scriptsServiceMock.Setup(x =>
                x.GetScriptContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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

    [ProviderState("create script with empty script language")]
    public void CreateScriptWithEmptyScriptType()
    {
        // handled by the validator   
    }

    [ProviderState("create script with invalid script language")]
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
            .ReturnsAsync(new ScriptDataResponse("id", "projectId", "filePath", "filePath", "srcPath",
                "scriptType", new List<ScriptParameterResponse>
                {
                    new("parameter name", "string", "parameter value")
                }));
    }

    [ProviderState("valid data to update a script")]
    public void UpdateScriptWithValidData()
    {
        _scriptsServiceMock.Setup(x => x.UpdateScriptAsync(It.IsAny<UpdateScript>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptDataResponse("id", "projectId", "filePath", "filePath", "srcPath",
                "scriptType", new List<ScriptParameterResponse>
                {
                    new("parameter name", "string", "parameter value")
                }));
    }

    [ProviderState("update script with project id for project that does not exist")]
    public void UpdateScriptForProjectIdThatDoesNotExist()
    {
        _scriptsServiceMock.Setup(x => x.UpdateScriptAsync(It.IsAny<UpdateScript>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectMissing("project-id"));
    }

    [ProviderState("update script for id that does not exist")]
    public void UpdateScriptForIdThatDoesNotExist()
    {
        _scriptsServiceMock.Setup(x => x.UpdateScriptAsync(It.IsAny<UpdateScript>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScriptMissing("script-id"));
    }

    [ProviderState("update script with empty project id")]
    public void UpdateScriptWithEmptyProjectId()
    {
        // handled by the validator   
    }

    [ProviderState("update script with empty script id")]
    public void UpdateScriptWithEmptyScriptId()
    {
        // handled by the validator
    }

    [ProviderState("error while updating script")]
    public void ErrorWhileUpdatingScript()
    {
        _scriptsServiceMock.Setup(x => x.UpdateScriptAsync(It.IsAny<UpdateScript>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TestException());
    }
}
