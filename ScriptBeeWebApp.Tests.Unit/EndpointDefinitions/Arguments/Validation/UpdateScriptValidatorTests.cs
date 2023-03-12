using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class UpdateScriptValidatorTests
{
    private readonly UpdateScriptValidator _validator;

    public UpdateScriptValidatorTests()
    {
        _validator = new UpdateScriptValidator();
    }

    [Fact]
    public async Task GivenValidCreateScript_WhenValidate_ThenResultHasNoErrors()
    {
        var createScript = new UpdateScript("path", "id1", new List<ScriptParameter>());

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var createScript = new UpdateScript("path", "", new List<ScriptParameter>());

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldHaveValidationErrorFor(r => r.ProjectId);
    }

    [Fact]
    public async Task GivenEmptyScriptId_WhenValidate_ThenResultHasErrors()
    {
        var createScript = new UpdateScript("", "projectId", new List<ScriptParameter>());

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public async Task GivenParametersWithEmptyName_WhenValidate_ThenResultHasErrors()
    {
        var createScript = new UpdateScript("path", "projectId", new List<ScriptParameter>
        {
            new("", "type", "value")
        });

        var result = await _validator.TestValidateAsync(createScript);

        result.ShouldHaveAnyValidationError();
    }
}
