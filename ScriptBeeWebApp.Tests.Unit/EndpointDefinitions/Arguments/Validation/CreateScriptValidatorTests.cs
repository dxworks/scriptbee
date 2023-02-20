using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class CreateScriptValidatorTests
{
    private readonly CreateScriptValidator _validator;

    public CreateScriptValidatorTests()
    {
        _validator = new CreateScriptValidator();
    }

    [Fact]
    public async Task GivenValidCreateScript_WhenValidate_ThenResultHasNoErrors()
    {
        var createScript = new CreateScript("id1", "path", "type", new List<ScriptParameter>());

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var createScript = new CreateScript("", "path", "type", new List<ScriptParameter>());

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldHaveValidationErrorFor(r => r.ProjectId);
    }

    [Fact]
    public async Task GivenEmptyFilePath_WhenValidate_ThenResultHasErrors()
    {
        var createScript = new CreateScript("projectId", "", "type", new List<ScriptParameter>());

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldHaveValidationErrorFor(r => r.FilePath);
    }

    [Fact]
    public async Task GivenEmptyScriptType_WhenValidate_ThenResultHasErrors()
    {
        var createScript = new CreateScript("projectId", "path", "", new List<ScriptParameter>());

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldHaveValidationErrorFor(r => r.ScriptLanguage);
    }

    [Fact]
    public async Task GivenEmptyParameters_WhenValidate_ThenResultHasNoErrors()
    {
        var createScript = new CreateScript("projectId", "path", "type", new List<ScriptParameter>());

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenParametersWithEmptyName_WhenValidate_ThenResultHasErrors()
    {
        var createScript = new CreateScript("projectId", "path", "type", new List<ScriptParameter>
        {
            new("", "type", "value")
        });

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldHaveAnyValidationError();
    }
}
