using FluentValidation.TestHelper;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Web.EndpointDefinitions.Context.Validation;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context.Validation;

public class LoadContextValidatorTest
{
    private readonly LoadContextValidator _linkContextValidator = new();

    [Fact]
    public async Task GivenValidCommand_ThenResultHasNoErrors()
    {
        var command = new WebLoadContextCommand(["linker-id"]);

        var result = await _linkContextValidator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenNullLoaderIds_ThenResultHasErrors()
    {
        var command = new WebLoadContextCommand(null!);

        var result = await _linkContextValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.LoaderIds)
            .WithErrorMessage("'Loader Ids' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyLoaderIds_ThenResultHasErrors()
    {
        var command = new WebLoadContextCommand([]);

        var result = await _linkContextValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.LoaderIds)
            .WithErrorMessage("'Loader Ids' must not be empty.");
    }
}
