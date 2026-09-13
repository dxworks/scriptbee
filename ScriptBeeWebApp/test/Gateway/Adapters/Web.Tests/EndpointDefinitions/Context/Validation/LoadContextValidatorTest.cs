using FluentValidation.TestHelper;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Web.EndpointDefinitions.Context.Validation;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context.Validation;

public class LoadContextValidatorTest
{
    private readonly LoadContextValidator _linkContextValidator = new();

    [Fact]
    public async Task GivenValidCommandWithLoaderIds_ThenResultHasNoErrors()
    {
        var command = new WebLoadContextCommand(["linker-id"], null);

        var result = await _linkContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenValidCommandWithFilesToLoad_ThenResultHasNoErrors()
    {
        var command = new WebLoadContextCommand(
            null,
            new Dictionary<string, List<string>> { { "loader-id", ["file-id"] } }
        );

        var result = await _linkContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenNeitherLoaderIdsNorFilesToLoad_ThenResultHasErrors()
    {
        var command = new WebLoadContextCommand(null, null);

        var result = await _linkContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.LoaderIds)
            .WithErrorMessage(
                "Either 'LoaderIds' or 'FilesToLoad' must be provided and non-empty."
            );
    }

    [Fact]
    public async Task GivenEmptyLoaderIdsAndNullFilesToLoad_ThenResultHasErrors()
    {
        var command = new WebLoadContextCommand([], null);

        var result = await _linkContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.LoaderIds)
            .WithErrorMessage(
                "Either 'LoaderIds' or 'FilesToLoad' must be provided and non-empty."
            );
    }
}
