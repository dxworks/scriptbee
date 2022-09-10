using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Controllers.Arguments.Validation;

public class GenerateScriptRequestValidatorTests
{
    private readonly GenerateScriptRequestValidator _generateScriptRequestValidator;

    public GenerateScriptRequestValidatorTests()
    {
        _generateScriptRequestValidator = new GenerateScriptRequestValidator();
    }

    [Fact]
    public async Task GivenValidGenerateScriptRequest_WhenValidate_ThenResultHasNoErrors()
    {
        var generateScriptRequest = new GenerateScriptRequest("id1", "type");

        var result = await _generateScriptRequestValidator.TestValidateAsync(generateScriptRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var generateScriptRequest = new GenerateScriptRequest("", "type");

        var result = await _generateScriptRequestValidator.TestValidateAsync(generateScriptRequest);

        result.ShouldHaveValidationErrorFor(r => r.ProjectId);
    }

    [Fact]
    public async Task GivenEmpty_WhenValidate_ThenResultHasErrors()
    {
        var generateScriptRequest = new GenerateScriptRequest("projectId", "");

        var result = await _generateScriptRequestValidator.TestValidateAsync(generateScriptRequest);

        result.ShouldHaveValidationErrorFor(r => r.ScriptType);
    }
}
