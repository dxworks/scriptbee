using FluentValidation.TestHelper;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Web.EndpointDefinitions.Context.Validation;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context.Validation;

public class LinkContextValidatorTest
{
    private readonly LinkContextValidator _linkContextValidator = new();

    [Fact]
    public async Task GivenValidCommand_ThenResultHasNoErrors()
    {
        var command = new WebLinkContextCommand(["linker-id"]);

        var result = await _linkContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenNullLinkerIds_ThenResultHasErrors()
    {
        var command = new WebLinkContextCommand(null!);

        var result = await _linkContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.LinkerIds)
            .WithErrorMessage("'Linker Ids' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyLinkerIds_ThenResultHasErrors()
    {
        var command = new WebLinkContextCommand([]);

        var result = await _linkContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.LinkerIds)
            .WithErrorMessage("'Linker Ids' must not be empty.");
    }
}
