// todo
// using System.Collections.Generic;
// using System.IO;
// using System.Threading;
// using System.Threading.Tasks;
// using AutoFixture;
// using DxWorks.ScriptBee.Plugin.Api;
// using FluentValidation;
// using FluentValidation.Results;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using ScriptBee.ProjectContext;
// using ScriptBeeWebApp.Controllers;
// using ScriptBeeWebApp.Controllers.Arguments;
// using ScriptBeeWebApp.Controllers.Arguments.Validation;
// using ScriptBeeWebApp.Services;
// using Xunit;
//
// namespace ScriptBeeWebApp.Tests.Unit.Controllers;
//
// public class GenerateScriptControllerTests
// {
//     private readonly Mock<IProjectManager> _projectManagerMock;
//     private readonly Mock<IGenerateScriptService> _generateScriptServiceMock;
//     private readonly Mock<IValidator<GenerateScriptRequest>> _generateScriptRequestValidatorMock;
//     private readonly Fixture _fixture;
//
//     private readonly GenerateScriptController _generateScriptController;
//
//     public GenerateScriptControllerTests()
//     {
//         _projectManagerMock = new Mock<IProjectManager>();
//         _generateScriptServiceMock = new Mock<IGenerateScriptService>();
//         _generateScriptRequestValidatorMock = new Mock<IValidator<GenerateScriptRequest>>();
//
//         _generateScriptController = new GenerateScriptController(_projectManagerMock.Object,
//             _generateScriptServiceMock.Object, _generateScriptRequestValidatorMock.Object);
//
//         _fixture = new Fixture();
//     }
//
//     [Fact]
//     public async Task GivenSupportedLanguages_WhenGetLanguages_ThenSupportedLanguagesAreReturned()
//     {
//         _generateScriptServiceMock.Setup(s => s.GetSupportedLanguages())
//             .Returns(new List<string> { "C#", "JavaScript" });
//
//         var actionResult = _generateScriptController.GetLanguages();
//         var okResult = (OkObjectResult)actionResult.Result!;
//
//         Assert.Equal(200, okResult.StatusCode);
//         Assert.Equal(new List<string> { "C#", "JavaScript" }, okResult.Value);
//     }
//
//     [Fact]
//     public async Task GivenInvalidGenerateScriptRequest_WhenPostGenerateScript_ThenBadRequestIsReturned()
//     {
//         var generateScriptRequest = _fixture.Create<GenerateScriptRequest>();
//         var expectedValidationErrorsResponse = new ValidationErrorsResponse(new List<ValidationError>
//         {
//             new("property", "error")
//         });
//
//         _generateScriptRequestValidatorMock
//             .Setup(v => v.ValidateAsync(generateScriptRequest, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("property", "error") }));
//
//         var actionResult = await _generateScriptController.PostGenerateScript(generateScriptRequest);
//         var badRequestResult = (BadRequestObjectResult)actionResult;
//         var validationErrorResponse = (ValidationErrorsResponse)badRequestResult.Value!;
//
//         Assert.Equal(400, badRequestResult.StatusCode);
//         Assert.Equal(expectedValidationErrorsResponse.Errors, validationErrorResponse.Errors);
//     }
//
//     [Fact]
//     public async Task GivenInvalidScriptType_WhenPostGenerateScript_ThenBadRequestIsReturned()
//     {
//         var generateScriptRequest = new GenerateScriptRequest("id1", "invalid");
//
//         _generateScriptRequestValidatorMock
//             .Setup(v => v.ValidateAsync(generateScriptRequest, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new ValidationResult());
//         _generateScriptServiceMock.Setup(s => s.GetGenerationStrategy("invalid"))
//             .Returns((IScriptGeneratorStrategy?)null);
//
//         var actionResult = await _generateScriptController.PostGenerateScript(generateScriptRequest);
//         var badRequestResult = (BadRequestObjectResult)actionResult;
//
//         Assert.Equal(400, badRequestResult.StatusCode);
//         Assert.Equal("Invalid script type", badRequestResult.Value);
//     }
//
//     [Fact]
//     public async Task GivenMissingProject_WhenPostGenerateScript_ThenNotFoundIsReturned()
//     {
//         var generateScriptRequest = new GenerateScriptRequest("id1", "valid");
//
//         _generateScriptRequestValidatorMock
//             .Setup(v => v.ValidateAsync(generateScriptRequest, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new ValidationResult());
//         _generateScriptServiceMock.Setup(s => s.GetGenerationStrategy("valid"))
//             .Returns(new Mock<IScriptGeneratorStrategy>().Object);
//         _projectManagerMock.Setup(p => p.GetProject("id1")).Returns((Project?)null);
//
//         var actionResult = await _generateScriptController.PostGenerateScript(generateScriptRequest);
//         var notFoundResult = (NotFoundObjectResult)actionResult;
//
//         Assert.Equal(404, notFoundResult.StatusCode);
//         Assert.Equal("Could not find project with id: id1", notFoundResult.Value);
//     }
//
//     [Fact]
//     public async Task GivenOkGenerateScriptRequest_WhenPostGenerateScript_ThenFileIsReturned()
//     {
//         var generateScriptRequest = new GenerateScriptRequest("id1", "valid");
//         var project = new Project();
//
//         var scriptGeneratorStrategyMock = new Mock<IScriptGeneratorStrategy>();
//         scriptGeneratorStrategyMock.Setup(s => s.Language).Returns("Language");
//
//         _generateScriptRequestValidatorMock
//             .Setup(v => v.ValidateAsync(generateScriptRequest, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new ValidationResult());
//         _generateScriptServiceMock.Setup(s => s.GetGenerationStrategy("valid"))
//             .Returns(scriptGeneratorStrategyMock.Object);
//         _projectManagerMock.Setup(p => p.GetProject("id1")).Returns(project);
//         _generateScriptServiceMock.Setup(s => s.GenerateClassesZip(It.IsAny<List<object>>(),
//                 It.IsAny<IScriptGeneratorStrategy>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(new Mock<Stream>().Object);
//
//         var actionResult = await _generateScriptController.PostGenerateScript(generateScriptRequest);
//         var result = (FileStreamResult)actionResult;
//
//         Assert.Equal("LanguageSampleCode.zip", result.FileDownloadName);
//         Assert.Equal("application/octet-stream", result.ContentType);
//     }
// }
