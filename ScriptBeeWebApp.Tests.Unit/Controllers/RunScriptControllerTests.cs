using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScriptBee.Models;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Controllers;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Controllers;

public class RunScriptControllerTests
{
    private readonly Mock<IProjectManager> _projectManagerMock;
    private readonly Mock<IProjectModelService> _projectModelServiceMock;
    private readonly Mock<IFileNameGenerator> _fileNameGeneratorMock;
    private readonly Mock<IRunScriptService> _runScriptServiceMock;
    private readonly Mock<IValidator<RunScript>> _runScriptValidatorMock;
    private readonly Fixture _fixture;

    private readonly RunScriptController _runScriptController;

    public RunScriptControllerTests()
    {
        _projectManagerMock = new Mock<IProjectManager>();
        _projectModelServiceMock = new Mock<IProjectModelService>();
        _fileNameGeneratorMock = new Mock<IFileNameGenerator>();
        _runScriptServiceMock = new Mock<IRunScriptService>();
        _runScriptValidatorMock = new Mock<IValidator<RunScript>>();

        _fixture = new Fixture();

        _runScriptController = new RunScriptController(_projectManagerMock.Object, _projectModelServiceMock.Object,
            _fileNameGeneratorMock.Object, _runScriptServiceMock.Object, _runScriptValidatorMock.Object);
    }

    [Fact]
    public void GivenLanguages_WhenGetLanguages_ThenReturnsLanguages()
    {
        var languages = new List<string> { "C#", "Python", "JavaScript" };
        _runScriptServiceMock.Setup(x => x.GetSupportedLanguages()).Returns(languages);

        var actionResult = _runScriptController.GetLanguages();
        var result = (OkObjectResult)actionResult.Result!;
        var resultValue = (List<string>)result.Value!;

        Assert.Equal(200, result.StatusCode);
        Assert.Equal(languages, resultValue);
    }

    [Fact]
    public async Task GivenInvalidRunScript_WhenRunScriptFromPath_ThenBadRequestIsReturned()
    {
        var runScript = _fixture.Create<RunScript>();
        var expectedValidationResult =
            new ValidationErrorsResponse(new List<ValidationError> { new("property", "error") });

        _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("property", "error") }));

        var actionResult = await _runScriptController.RunScriptFromPath(runScript);
        var result = (BadRequestObjectResult)actionResult;
        var validationErrorResponse = (ValidationErrorsResponse)result.Value!;

        Assert.Equal(400, result.StatusCode);
        Assert.Equal(expectedValidationResult.Errors, validationErrorResponse.Errors);
    }

    [Fact]
    public async Task GivenMissingProject_WhenRunScriptFromPath_ThenNotFoundIsReturned()
    {
        var runScript = _fixture.Create<RunScript>();

        _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
            .ReturnsAsync(new ValidationResult());
        _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
            .Returns((Project?)null);

        var actionResult = await _runScriptController.RunScriptFromPath(runScript);
        var result = (NotFoundObjectResult)actionResult;

        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"Could not find project with id: {runScript.ProjectId}", result.Value);
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

        var actionResult = await _runScriptController.RunScriptFromPath(runScript);
        var result = (NotFoundObjectResult)actionResult;

        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"Could not find project model with id: {runScript.ProjectId}", result.Value);
    }

    [Fact]
    public async Task GivenMissingFilePathWhileRunning_WhenRunScriptFromPath_ThenNotFoundIsReturned()
    {
        var runScript = _fixture.Create<RunScript>();
        var context = _fixture.Create<Context>();
        var project = _fixture.Build<Project>()
            .With(p => p.Context, context)
            .Create();
        var projectModel = _fixture.Create<ProjectModel>();

        _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
            .ReturnsAsync(new ValidationResult());
        _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
            .Returns(project);
        _projectModelServiceMock.Setup(x => x.GetDocument(runScript.ProjectId, default))
            .ReturnsAsync(projectModel);
        _runScriptServiceMock.Setup(x => x.RunAsync(project, projectModel, runScript.Language, runScript.FilePath,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RunModel?)null);

        var actionResult = await _runScriptController.RunScriptFromPath(runScript);
        var result = (NotFoundObjectResult)actionResult;

        Assert.Equal(404, result.StatusCode);
        Assert.Equal($"File from {runScript.FilePath} not found", result.Value);
    }


    [Fact]
    public async Task GivenValidRunScript_WhenRunScriptFromPath_ThenOkIsReturned()
    {
        var runScript = _fixture.Create<RunScript>();
        var context = _fixture.Create<Context>();
        var project = _fixture.Build<Project>()
            .With(p => p.Context, context)
            .Create();
        var projectModel = _fixture.Create<ProjectModel>();
        var runModel = _fixture.Build<RunModel>()
            .With(r => r.OutputFileNames, new List<string> { "output" })
            .Create();

        var expectedOutputFiles = new List<OutputFile>
        {
            new("fileName", "outputType", "output")
        };

        _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
            .ReturnsAsync(new ValidationResult());
        _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
            .Returns(project);
        _projectModelServiceMock.Setup(x => x.GetDocument(runScript.ProjectId, default))
            .ReturnsAsync(projectModel);
        _runScriptServiceMock.Setup(x =>
                x.RunAsync(project, projectModel, runScript.Language, runScript.FilePath,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(runModel);
        _fileNameGeneratorMock.Setup(x => x.ExtractOutputFileNameComponents("output"))
            .Returns((runModel.ProjectId, runModel.Id, "outputType", "fileName"));

        var actionResult = await _runScriptController.RunScriptFromPath(runScript);
        var result = (OkObjectResult)actionResult;
        var returnedRun = (ReturnedRun)result.Value!;

        Assert.Equal(200, result.StatusCode);
        Assert.Equal(runModel.Id, returnedRun.RunId);
        Assert.Equal(runModel.ProjectId, returnedRun.ProjectId);
        Assert.Equal(runModel.RunIndex, returnedRun.RunIndex);
        Assert.Equal(runModel.ConsoleOutputName, returnedRun.ConsoleOutputName);
        Assert.Equal(runModel.Errors, returnedRun.Errors);
        Assert.Equal(expectedOutputFiles, returnedRun.OutputFiles);
    }
}
