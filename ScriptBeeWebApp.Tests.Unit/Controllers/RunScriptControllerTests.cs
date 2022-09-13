// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using AutoFixture;
// using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;
// using DxWorks.ScriptBee.Plugin.Api.ScriptRunner;
// using FluentValidation;
// using FluentValidation.Results;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using ScriptBee.Models;
// using ScriptBee.Plugin;
// using ScriptBee.ProjectContext;
// using ScriptBeeWebApp.Controllers;
// using ScriptBeeWebApp.Controllers.Arguments;
// using ScriptBeeWebApp.Controllers.Arguments.Validation;
// using ScriptBeeWebApp.Services;
// using Xunit;
//
// namespace ScriptBeeWebApp.Tests.Unit.Controllers;
//
// public class RunScriptControllerTests
// {
//     private readonly Mock<IProjectManager> _projectManagerMock;
//     private readonly Mock<IProjectModelService> _projectModelServiceMock;
//     private readonly Mock<IFileNameGenerator> _fileNameGeneratorMock;
//     private readonly Mock<IRunScriptService> _runScriptServiceMock;
//     private readonly Mock<IPluginRepository> _pluginRepositoryMock;
//     private readonly Mock<IValidator<RunScript>> _runScriptValidatorMock;
//     private readonly Fixture _fixture;
//
//     private readonly RunScriptController _runScriptController;
//
//     public RunScriptControllerTests()
//     {
//         _projectManagerMock = new Mock<IProjectManager>();
//         _projectModelServiceMock = new Mock<IProjectModelService>();
//         _fileNameGeneratorMock = new Mock<IFileNameGenerator>();
//         _runScriptServiceMock = new Mock<IRunScriptService>();
//         _pluginRepositoryMock = new Mock<IPluginRepository>();
//         _runScriptValidatorMock = new Mock<IValidator<RunScript>>();
//
//         _runScriptController = new RunScriptController(_projectManagerMock.Object, _projectModelServiceMock.Object,
//             _fileNameGeneratorMock.Object, _runScriptServiceMock.Object, _pluginRepositoryMock.Object,
//             _runScriptValidatorMock.Object);
//
//         _fixture = new Fixture();
//     }
//
//     [Fact]
//     public async Task GivenInvalidRunScript_WhenRunScriptFromPath_ThenBadRequestIsReturned()
//     {
//         var runScript = _fixture.Create<RunScript>();
//         var expectedValidationResult =
//             new ValidationErrorsResponse(new List<ValidationError> { new("property", "error") });
//
//         _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
//             .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("property", "error") }));
//
//         var actionResult = await _runScriptController.RunScriptFromPath(runScript);
//         var result = (BadRequestObjectResult)actionResult;
//         var validationErrorResponse = (ValidationErrorsResponse)result.Value!;
//
//         Assert.Equal(400, result.StatusCode);
//         Assert.Equal(expectedValidationResult.Errors, validationErrorResponse.Errors);
//     }
//
//     [Fact]
//     public async Task GivenInvalidLanguage_WhenRunScriptFromPath_ThenBadRequestIsReturned()
//     {
//         var runScript = _fixture.Create<RunScript>();
//
//         _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
//             .ReturnsAsync(new ValidationResult());
//         _pluginRepositoryMock.Setup(x => x.GetPlugin<IScriptRunner>(runScript.Language))
//             .Returns((IScriptRunner?)null);
//
//         var actionResult = await _runScriptController.RunScriptFromPath(runScript);
//         var result = (BadRequestObjectResult)actionResult;
//
//         Assert.Equal(400, result.StatusCode);
//         Assert.Equal($"ScriptRunner for language {runScript.Language} not supported", result.Value);
//     }
//
//     [Fact]
//     public async Task GivenMissingProject_WhenRunScriptFromPath_ThenNotFoundIsReturned()
//     {
//         var runScript = _fixture.Create<RunScript>();
//
//         _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
//             .ReturnsAsync(new ValidationResult());
//         _pluginRepositoryMock.Setup(x => x.GetPlugin<IScriptRunner>(runScript.Language))
//             .Returns(new Mock<IScriptRunner>().Object);
//         _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
//             .Returns((Project?)null);
//
//         var actionResult = await _runScriptController.RunScriptFromPath(runScript);
//         var result = (NotFoundObjectResult)actionResult;
//
//         Assert.Equal(404, result.StatusCode);
//         Assert.Equal($"Could not find project with id: {runScript.ProjectId}", result.Value);
//     }
//
//     [Fact]
//     public async Task GivenMissingProjectModel_WhenRunScriptFromPath_ThenNotFoundIsReturned()
//     {
//         var runScript = _fixture.Create<RunScript>();
//         var context = _fixture.Create<Context>();
//         var project = _fixture.Build<Project>()
//             .With(p => p.Context, context)
//             .Create();
//
//         _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
//             .ReturnsAsync(new ValidationResult());
//         _pluginRepositoryMock.Setup(x => x.GetPlugin<IScriptRunner>(runScript.Language))
//             .Returns(new Mock<IScriptRunner>().Object);
//         _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
//             .Returns(project);
//         _projectModelServiceMock.Setup(x => x.GetDocument(project.Id, default))
//             .ReturnsAsync((ProjectModel?)null);
//
//         var actionResult = await _runScriptController.RunScriptFromPath(runScript);
//         var result = (NotFoundObjectResult)actionResult;
//
//         Assert.Equal(404, result.StatusCode);
//         Assert.Equal($"Could not find project model with id: {runScript.ProjectId}", result.Value);
//     }
//
//     [Fact]
//     public async Task GivenMissingFilePathWhileRunning_WhenRunScriptFromPath_ThenNotFoundIsReturned()
//     {
//         var runScript = _fixture.Create<RunScript>();
//         var context = _fixture.Create<Context>();
//         var project = _fixture.Build<Project>()
//             .With(p => p.Context, context)
//             .Create();
//         var projectModel = _fixture.Create<ProjectModel>();
//
//         var scriptRunner = new Mock<IScriptRunner>().Object;
//
//         _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
//             .ReturnsAsync(new ValidationResult());
//         _pluginRepositoryMock.Setup(x => x.GetPlugin<IScriptRunner>(runScript.Language))
//             .Returns(scriptRunner);
//         _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
//             .Returns(project);
//         _projectModelServiceMock.Setup(x => x.GetDocument(runScript.ProjectId, default))
//             .ReturnsAsync(projectModel);
//         _runScriptServiceMock.Setup(x =>
//                 x.RunAsync(scriptRunner, project, projectModel, runScript.FilePath, It.IsAny<CancellationToken>()))
//             .ReturnsAsync((RunModel?)null);
//
//         var actionResult = await _runScriptController.RunScriptFromPath(runScript);
//         var result = (NotFoundObjectResult)actionResult;
//
//         Assert.Equal(404, result.StatusCode);
//         Assert.Equal($"File from {runScript.FilePath} not found", result.Value);
//     }
//
//     [Fact]
//     public async Task GivenValidRunScript_WhenRunScriptFromPath_ThenOkIsReturned()
//     {
//         var runScript = _fixture.Create<RunScript>();
//         var context = _fixture.Create<Context>();
//         var project = _fixture.Build<Project>()
//             .With(p => p.Context, context)
//             .Create();
//         var projectModel = _fixture.Create<ProjectModel>();
//         var runModel = _fixture.Build<RunModel>()
//             .With(r => r.OutputFileNames, new List<string> { "output" })
//             .Create();
//
//         var scriptRunner = new Mock<IScriptRunner>().Object;
//         var expectedOutputFiles = new List<OutputFile>
//         {
//             new("fileName", "outputType", "output")
//         };
//
//         _runScriptValidatorMock.Setup(x => x.ValidateAsync(runScript, default))
//             .ReturnsAsync(new ValidationResult());
//         _pluginRepositoryMock.Setup(x => x.GetPlugin<IScriptRunner>(runScript.Language))
//             .Returns(scriptRunner);
//         _projectManagerMock.Setup(x => x.GetProject(runScript.ProjectId))
//             .Returns(project);
//         _projectModelServiceMock.Setup(x => x.GetDocument(runScript.ProjectId, default))
//             .ReturnsAsync(projectModel);
//         _runScriptServiceMock.Setup(x =>
//                 x.RunAsync(scriptRunner, project, projectModel, runScript.FilePath, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(runModel);
//         _fileNameGeneratorMock.Setup(x => x.ExtractOutputFileNameComponents("output"))
//             .Returns((runModel.ProjectId, runModel.Id, "outputType", "fileName"));
//
//         var actionResult = await _runScriptController.RunScriptFromPath(runScript);
//         var result = (OkObjectResult)actionResult;
//         var returnedRun = (ReturnedRun)result.Value!;
//
//         Assert.Equal(200, result.StatusCode);
//         Assert.Equal(runModel.Id, returnedRun.RunId);
//         Assert.Equal(runModel.ProjectId, returnedRun.ProjectId);
//         Assert.Equal(runModel.RunIndex, returnedRun.RunIndex);
//         Assert.Equal(runModel.ConsoleOutputName, returnedRun.ConsoleOutputName);
//         Assert.Equal(runModel.Errors, returnedRun.Errors);
//         Assert.Equal(expectedOutputFiles, returnedRun.OutputFiles);
//     }
// }
