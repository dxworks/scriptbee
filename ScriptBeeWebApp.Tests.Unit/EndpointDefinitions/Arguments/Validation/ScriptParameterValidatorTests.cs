using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBee.Models;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class ScriptParameterValidatorTests
{
    private readonly ScriptParameterValidator _validator;

    public ScriptParameterValidatorTests()
    {
        _validator = new ScriptParameterValidator();
    }

    [Fact]
    public async Task GivenValidScriptParameter_WhenValidate_ThenResultHasNoErrors()
    {
        var scriptParameter = new ScriptParameter("name", ScriptParameterModel.TypeString, "value");

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyName_WhenValidate_ThenResultHasErrors()
    {
        var scriptParameter = new ScriptParameter("", ScriptParameterModel.TypeString, "value");

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public async Task GivenInvalidType_WhenValidate_ThenResultHasErrors()
    {
        var scriptParameter = new ScriptParameter("name", "invalidType", "value");

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldHaveValidationErrorFor(r => r.Type);
    }

    [Theory]
    [InlineData(ScriptParameterModel.TypeString)]
    [InlineData(ScriptParameterModel.TypeInteger)]
    [InlineData(ScriptParameterModel.TypeBoolean)]
    [InlineData(ScriptParameterModel.TypeFloat)]
    public async Task GivenValidType_WhenValidate_ThenResultHasNoErrors(string type)
    {
        var scriptParameter = new ScriptParameter("name", type, "value");

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenNullValue_WhenValidate_ThenResultHasNoErrors()
    {
        var scriptParameter = new ScriptParameter("name", ScriptParameterModel.TypeString, null);

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
