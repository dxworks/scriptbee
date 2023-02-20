using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Repository;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions;

// todo to be replaced by Pact tests
public class RunScriptEndpointDefinitionTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IProjectManager> _projectManagerMock;
    private readonly Mock<IProjectModelService> _projectModelServiceMock;
    private readonly Mock<IRunScriptService> _runScriptServiceMock;
    private readonly Mock<IValidator<RunScript>> _runScriptValidatorMock;

    public RunScriptEndpointDefinitionTests()
    {
        _projectManagerMock = new Mock<IProjectManager>();
        _projectModelServiceMock = new Mock<IProjectModelService>();
        _runScriptServiceMock = new Mock<IRunScriptService>();
        _runScriptValidatorMock = new Mock<IValidator<RunScript>>();

        _fixture = new Fixture();
    }

    [Fact]
    public void GivenLanguages_WhenGetLanguages_ThenReturnsLanguages()
    {
        var expectedLanguages = new List<string> { "C#", "Python", "JavaScript" };
        _runScriptServiceMock.Setup(x => x.GetSupportedLanguages()).Returns(expectedLanguages);

        var languages = RunScriptEndpointDefinition.GetLanguages(_runScriptServiceMock.Object);

        Assert.Equal(expectedLanguages, languages);
    }

    [Fact]
    public async Task GivenInvalidRunScript_WhenRunScriptFromPath_ThenBadRequestIsReturned()
    {
        var runScript = _fixture.Create<RunScript>();
        var expectedValidationResult =
            new ValidationErrorsResponse(new List<ValidationError> { new("property", "error") });

        _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("property", "error") }));

        var result = await RunScriptEndpointDefinition.RunScriptFromPath(runScript,
            _runScriptValidatorMock.Object, _runScriptServiceMock.Object, _projectManagerMock.Object,
            _projectModelServiceMock.Object);

        Assert.Equal(400, result.GetStatusCode());
        Assert.Equal(expectedValidationResult.Errors, result.GetValue<ValidationErrorsResponse>().Errors);
    }

    [Fact]
    public async Task GivenMissingProject_WhenRunScriptFromPath_ThenNotFoundIsReturned()
    {
        var runScript = _fixture.Create<RunScript>();

        _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
            .ReturnsAsync(new ValidationResult());
        _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
            .Returns((Project?)null);

        var result = await RunScriptEndpointDefinition.RunScriptFromPath(runScript,
            _runScriptValidatorMock.Object, _runScriptServiceMock.Object, _projectManagerMock.Object,
            _projectModelServiceMock.Object);

        Assert.Equal(404, result.GetStatusCode());
        Assert.Equal($"Could not find project with id: {runScript.ProjectId}", result.GetValue<string>());
    }

    [Fact]
    public async Task GivenMissingProjectModel_WhenRunScriptFromPath_ThenNotFoundIsReturned()
    {
        var runScript = _fixture.Create<RunScript>();
        var context = _fixture.Create<Context>();
        var project = _fixture.Build<Project>()
            .With(p => p.Context, context)
            .Create();

        _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
            .ReturnsAsync(new ValidationResult());
        _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
            .Returns(project);
        _projectModelServiceMock.Setup(x => x.GetDocument(project.Id, default))
            .ReturnsAsync((ProjectModel?)null);

        var result = await RunScriptEndpointDefinition.RunScriptFromPath(runScript,
            _runScriptValidatorMock.Object, _runScriptServiceMock.Object, _projectManagerMock.Object,
            _projectModelServiceMock.Object);

        Assert.Equal(404, result.GetStatusCode());
        Assert.Equal($"Could not find project model with id: {runScript.ProjectId}", result.GetValue<string>());
    }

    // todo add tests for remapped exception thrown by runScriptService 

    [Fact]
    public async Task GivenValidRunScript_WhenRunScriptFromPath_ThenOkIsReturned()
    {
        var runScript = _fixture.Create<RunScript>();
        var context = _fixture.Create<Context>();
        var project = _fixture.Build<Project>()
            .With(p => p.Context, context)
            .Create();
        var projectModel = _fixture.Create<ProjectModel>();
        var run = _fixture.Create<Run>();

        _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
            .ReturnsAsync(new ValidationResult());
        _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
            .Returns(project);
        _projectModelServiceMock.Setup(x => x.GetDocument(runScript.ProjectId, default))
            .ReturnsAsync(projectModel);
        _runScriptServiceMock.Setup(x =>
                x.RunAsync(project, projectModel, runScript.Language, runScript.FilePath,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(run);

        var result = await RunScriptEndpointDefinition.RunScriptFromPath(runScript,
            _runScriptValidatorMock.Object, _runScriptServiceMock.Object, _projectManagerMock.Object,
            _projectModelServiceMock.Object);

        var returnedRun = result.GetValue<ReturnedRun>();

        Assert.Equal(200, result.GetStatusCode());
        Assert.Equal(run.Index, returnedRun.Index);
        Assert.Equal(run.ScriptPath, returnedRun.ScriptName);
        Assert.Equal(run.Linker, returnedRun.Linker);
        Assert.Equal(run.LoadedFiles.Count, returnedRun.LoadedFiles.Count);
        foreach (var (key, files) in run.LoadedFiles)
        {
            Assert.Equal(files.Count, returnedRun.LoadedFiles[key].Count);
            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var loadedFile = returnedRun.LoadedFiles[key][i];
                Assert.Equal(file.Name, loadedFile);
            }
        }

        Assert.Equal(run.Results.Count, returnedRun.Results.Count);
        for (var i = 0; i < run.Results.Count; i++)
        {
            var runResult = run.Results[i];
            var r = returnedRun.Results[i];

            Assert.Equal(runResult.Id, r.Id);
            Assert.Equal(runResult.Name, r.Name);
            Assert.Equal(runResult.Type, r.Type);
        }
    }
}
