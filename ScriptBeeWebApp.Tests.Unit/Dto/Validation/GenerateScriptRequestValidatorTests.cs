using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.Dto;
using ScriptBeeWebApp.Dto.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Dto.Validation;

public class GenerateScriptRequestValidatorTests
{
    private readonly GenerateScriptRequestValidator _generateScriptRequestValidator;

    public GenerateScriptRequestValidatorTests()
    {
        _generateScriptRequestValidator = new GenerateScriptRequestValidator();
    }

    [Fact]
    public async Task GivenValidGenerateScriptRequest_WhenValidating_ThenResultHasNoErrors()
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
