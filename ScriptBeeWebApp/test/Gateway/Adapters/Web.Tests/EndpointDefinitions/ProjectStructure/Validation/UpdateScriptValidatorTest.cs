using FluentValidation.TestHelper;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Validation;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure.Validation;

public class UpdateScriptValidatorTest
{
    private readonly UpdateScriptValidator _validator = new();

    [Theory]
    [InlineData("string")]
    [InlineData("integer")]
    [InlineData("float")]
    [InlineData("boolean")]
    public async Task GivenValidUpdateScript_ThenResultHasNoErrors(string type)
    {
        var createProject = new WebUpdateScriptCommand([
            new WebScriptParameter("parameter", type, "value"),
        ]);

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenValidCreateScriptWithNullParameters_ThenResultHasNoErrors()
    {
        var createProject = new WebUpdateScriptCommand(null);

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenValidCreateScriptWithEmptyParameters_ThenResultHasNoErrors()
    {
        var createProject = new WebUpdateScriptCommand([]);

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyParameterName_ThenResultHasErrors()
    {
        var createProject = new WebUpdateScriptCommand([
            new WebScriptParameter("", "string", "value"),
        ]);

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor("Parameters[0].Name")
            .WithErrorMessage("'Name' must not be empty.");
    }

    [Fact]
    public async Task GivenNullParameterName_ThenResultHasErrors()
    {
        var createProject = new WebUpdateScriptCommand([
            new WebScriptParameter(null!, "string", "value"),
        ]);

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Name")
            .WithErrorMessage("'Name' must not be empty.");
    }

    [Fact]
    public async Task GivenNullParameterType_ThenResultHasErrors()
    {
        var createProject = new WebUpdateScriptCommand([
            new WebScriptParameter("parameter", null!, "value"),
        ]);

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Type")
            .WithErrorMessage("'Type' must not be empty.");
    }

    [Fact]
    public async Task GivenInvalidParameterType_ThenResultHasErrors()
    {
        var createProject = new WebUpdateScriptCommand([
            new WebScriptParameter("parameter", "invalid", "value"),
        ]);

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Type")
            .WithErrorMessage(
                "'Type' must be one of the following values: string,integer,float,boolean."
            );
    }

    [Fact]
    public async Task GivenInvalidFields_ThenResultHasErrors()
    {
        var createProject = new WebUpdateScriptCommand([
            new WebScriptParameter(null!, null!, null),
        ]);

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor("Parameters[0].Name")
            .WithErrorMessage("'Name' must not be empty.");
        result
            .ShouldHaveValidationErrorFor("Parameters[0].Type")
            .WithErrorMessage("'Type' must not be empty.");
    }
}
