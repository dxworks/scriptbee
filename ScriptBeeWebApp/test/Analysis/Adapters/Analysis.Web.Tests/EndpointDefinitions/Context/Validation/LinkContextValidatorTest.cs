using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Validation;
using FluentValidation.TestHelper;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context.Validation;

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
    public async Task GivenEmptyLinkerIds_ThenResultHasNoErrors()
    {
        var command = new WebLinkContextCommand([]);

        var result = await _linkContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }
}
