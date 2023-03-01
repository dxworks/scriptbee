using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.Model;
using FluentValidation.TestHelper;
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
        var scriptParameter = new ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter("name", ScriptParameter.TypeString, "value");

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyName_WhenValidate_ThenResultHasErrors()
    {
        var scriptParameter = new ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter("", ScriptParameter.TypeString, "value");

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public async Task GivenInvalidType_WhenValidate_ThenResultHasErrors()
    {
        var scriptParameter = new ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter("name", "invalidType", "value");

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldHaveValidationErrorFor(r => r.Type);
    }

    [Theory]
    [InlineData(ScriptParameter.TypeString)]
    [InlineData(ScriptParameter.TypeInteger)]
    [InlineData(ScriptParameter.TypeBoolean)]
    [InlineData(ScriptParameter.TypeFloat)]
    public async Task GivenValidType_WhenValidate_ThenResultHasNoErrors(string type)
    {
        var scriptParameter = new ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter("name", type, "value");

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenNullValue_WhenValidate_ThenResultHasNoErrors()
    {
        var scriptParameter = new ScriptBeeWebApp.EndpointDefinitions.Arguments.ScriptParameter("name", ScriptParameter.TypeString, null);

        var result = await _validator.TestValidateAsync(scriptParameter);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
