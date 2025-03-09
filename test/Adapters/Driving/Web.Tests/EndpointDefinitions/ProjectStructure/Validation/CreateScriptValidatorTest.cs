using FluentValidation.TestHelper;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Validation;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure.Validation;

public class CreateScriptValidatorTest
{
    private readonly CreateScriptValidator _createScriptValidator = new();

    [Theory]
    [InlineData("string")]
    [InlineData("integer")]
    [InlineData("float")]
    [InlineData("boolean")]
    public async Task GivenValidCreateScript_ThenResultHasNoErrors(string type)
    {
        var createProject = new WebCreateScriptCommand(
            "path",
            "csharp",
            [new WebScriptParameter("parameter", type, "value")]
        );

        var result = await _createScriptValidator.TestValidateAsync(createProject);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyPath_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand("", "language", null);

        var result = await _createScriptValidator.TestValidateAsync(createProject);

        result
            .ShouldHaveValidationErrorFor(x => x.Path)
            .WithErrorMessage("'Path' must not be empty.");
    }

    [Fact]
    public async Task GivenNullPath_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand(null!, "language", null);

        var result = await _createScriptValidator.TestValidateAsync(createProject);

        result
            .ShouldHaveValidationErrorFor(x => x.Path)
            .WithErrorMessage("'Path' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyLanguage_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand("path", "", null);

        var result = await _createScriptValidator.TestValidateAsync(createProject);

        result
            .ShouldHaveValidationErrorFor(x => x.Language)
            .WithErrorMessage("'Language' must not be empty.");
    }

    [Fact]
    public async Task GivenNullLanguage_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand("path", null!, null);

        var result = await _createScriptValidator.TestValidateAsync(createProject);

        result
            .ShouldHaveValidationErrorFor(x => x.Language)
            .WithErrorMessage("'Language' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyParameterName_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand(
            "path",
            "language",
            [new WebScriptParameter("", "string", "value")]
        );

        var result = await _createScriptValidator.TestValidateAsync(createProject);

        result
            .ShouldHaveValidationErrorFor("Parameters[0].Name")
            .WithErrorMessage("'Name' must not be empty.");
    }

    [Fact]
    public async Task GivenNullParameterName_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand(
            "path",
            "language",
            [new WebScriptParameter(null!, "string", "value")]
        );

        var result = await _createScriptValidator.TestValidateAsync(createProject);
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Name")
            .WithErrorMessage("'Name' must not be empty.");
    }

    [Fact]
    public async Task GivenNullParameterType_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand(
            "path",
            "language",
            [new WebScriptParameter("parameter", null!, "value")]
        );

        var result = await _createScriptValidator.TestValidateAsync(createProject);
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Type")
            .WithErrorMessage("'Type' must not be empty.");
    }

    [Fact]
    public async Task GivenInvalidParameterType_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand(
            "path",
            "language",
            [new WebScriptParameter("parameter", "invalid", "value")]
        );

        var result = await _createScriptValidator.TestValidateAsync(createProject);
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Type")
            .WithErrorMessage(
                "'Type' must be one of the following values: string,integer,float,boolean."
            );
    }

    [Fact]
    public async Task GivenInvalidFields_ThenResultHasErrors()
    {
        var createProject = new WebCreateScriptCommand(
            null!,
            null!,
            [new WebScriptParameter(null!, null!, null)]
        );

        var result = await _createScriptValidator.TestValidateAsync(createProject);

        result
            .ShouldHaveValidationErrorFor(x => x.Path)
            .WithErrorMessage("'Path' must not be empty.");
        result
            .ShouldHaveValidationErrorFor(x => x.Language)
            .WithErrorMessage("'Language' must not be empty.");
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Name")
            .WithErrorMessage("'Name' must not be empty.");
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Type")
            .WithErrorMessage("'Type' must not be empty.");
    }
}
