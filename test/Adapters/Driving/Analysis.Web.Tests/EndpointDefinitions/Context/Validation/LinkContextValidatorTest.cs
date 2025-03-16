using FluentValidation.TestHelper;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Validation;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context.Validation;

public class LinkContextValidatorTest
{
    private readonly LinkContextValidator _linkContextValidator = new();

    [Fact]
    public async Task GivenValidCommand_ThenResultHasNoErrors()
    {
        var command = new WebLinkContextCommand(["linker-id"]);

        var result = await _linkContextValidator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenNullLinkerIds_ThenResultHasErrors()
    {
        var command = new WebLinkContextCommand(null!);

        var result = await _linkContextValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.LinkerIds)
            .WithErrorMessage("'Linker Ids' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyLinkerIds_ThenResultHasErrors()
    {
        var command = new WebLinkContextCommand([]);

        var result = await _linkContextValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.LinkerIds)
            .WithErrorMessage("'Linker Ids' must not be empty.");
    }
}
