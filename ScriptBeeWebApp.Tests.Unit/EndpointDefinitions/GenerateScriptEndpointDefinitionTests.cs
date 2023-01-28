using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using DxWorks.ScriptBee.Plugin.Api;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions;

// todo to be replaced by Pact tests
public class GenerateScriptEndpointDefinitionTests
{
    private readonly Fixture _fixture;

    private readonly Mock<IValidator<GenerateScriptRequest>> _generateScriptRequestValidatorMock;
    private readonly Mock<IGenerateScriptService> _generateScriptServiceMock;
    private readonly Mock<IProjectManager> _projectManagerMock;

    public GenerateScriptEndpointDefinitionTests()
    {
        _projectManagerMock = new Mock<IProjectManager>();
        _generateScriptServiceMock = new Mock<IGenerateScriptService>();
        _generateScriptRequestValidatorMock = new Mock<IValidator<GenerateScriptRequest>>();

        _fixture = new Fixture();
    }

    [Fact]
    public void GivenSupportedLanguages_WhenGetLanguages_ThenSupportedLanguagesAreReturned()
    {
        _generateScriptServiceMock.Setup(s => s.GetSupportedLanguages())
            .Returns(new List<string> { "C#", "JavaScript" });

        var languages = GenerateScriptEndpointDefinition.GetLanguages(_generateScriptServiceMock.Object);

        Assert.Equal(new List<string> { "C#", "JavaScript" }, languages);
    }

    [Fact]
    public async Task GivenInvalidGenerateScriptRequest_WhenPostGenerateScript_ThenBadRequestIsReturned()
    {
        var generateScriptRequest = _fixture.Create<GenerateScriptRequest>();
        var expectedValidationErrorsResponse = new ValidationErrorsResponse(new List<ValidationError>
        {
            new("property", "error")
        });

        _generateScriptRequestValidatorMock
            .Setup(v => v.ValidateAsync(generateScriptRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("property", "error") }));

        var result = await GenerateScriptEndpointDefinition.PostGenerateScript(generateScriptRequest,
            _generateScriptRequestValidatorMock.Object, _generateScriptServiceMock.Object, _projectManagerMock.Object);

        Assert.Equal(400, result.GetStatusCode());
        Assert.Equal(expectedValidationErrorsResponse.Errors, result.GetValue<ValidationErrorsResponse>().Errors);
    }

    [Fact]
    public async Task GivenInvalidScriptType_WhenPostGenerateScript_ThenBadRequestIsReturned()
    {
        var generateScriptRequest = new GenerateScriptRequest("id1", "invalid");

        _generateScriptRequestValidatorMock
            .Setup(v => v.ValidateAsync(generateScriptRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _generateScriptServiceMock.Setup(s => s.GetGenerationStrategy("invalid"))
            .Returns((IScriptGeneratorStrategy?)null);

        var result = await GenerateScriptEndpointDefinition.PostGenerateScript(generateScriptRequest,
            _generateScriptRequestValidatorMock.Object, _generateScriptServiceMock.Object, _projectManagerMock.Object);

        Assert.Equal(400, result.GetStatusCode());
        Assert.Equal("Invalid script type", result.GetValue<string>());
    }

    [Fact]
    public async Task GivenMissingProject_WhenPostGenerateScript_ThenNotFoundIsReturned()
    {
        var generateScriptRequest = new GenerateScriptRequest("id1", "valid");

        _generateScriptRequestValidatorMock
            .Setup(v => v.ValidateAsync(generateScriptRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _generateScriptServiceMock.Setup(s => s.GetGenerationStrategy("valid"))
            .Returns(new Mock<IScriptGeneratorStrategy>().Object);
        _projectManagerMock.Setup(p => p.GetProject("id1")).Returns((Project?)null);

        var result = await GenerateScriptEndpointDefinition.PostGenerateScript(generateScriptRequest,
            _generateScriptRequestValidatorMock.Object, _generateScriptServiceMock.Object, _projectManagerMock.Object);

        Assert.Equal(404, result.GetStatusCode());
        Assert.Equal("Could not find project with id: id1", result.GetValue<string>());
    }

    [Fact]
    public async Task GivenOkGenerateScriptRequest_WhenPostGenerateScript_ThenFileIsReturned()
    {
        var generateScriptRequest = new GenerateScriptRequest("id1", "valid");
        var project = new Project();

        var scriptGeneratorStrategyMock = new Mock<IScriptGeneratorStrategy>();
        scriptGeneratorStrategyMock.Setup(s => s.Language).Returns("Language");

        _generateScriptRequestValidatorMock
            .Setup(v => v.ValidateAsync(generateScriptRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _generateScriptServiceMock.Setup(s => s.GetGenerationStrategy("valid"))
            .Returns(scriptGeneratorStrategyMock.Object);
        _projectManagerMock.Setup(p => p.GetProject("id1")).Returns(project);
        _generateScriptServiceMock.Setup(s => s.GenerateClassesZip(It.IsAny<List<object>>(),
                It.IsAny<IScriptGeneratorStrategy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<Stream>().Object);

        var result = await GenerateScriptEndpointDefinition.PostGenerateScript(generateScriptRequest,
            _generateScriptRequestValidatorMock.Object, _generateScriptServiceMock.Object, _projectManagerMock.Object);

        Assert.Equal("validSampleCode.zip", result.GetProperty<string>("FileDownloadName"));
        Assert.Equal("application/octet-stream", result.GetProperty<string>("ContentType"));
    }
}
