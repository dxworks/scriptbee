using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Validation;
using FluentValidation.TestHelper;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context.Validation;

public class LoadContextValidatorTest
{
    private readonly LoadContextValidator _loadContextValidator = new();

    [Fact]
    public async Task GivenValidCommand_ThenResultHasNoErrors()
    {
        var command = new WebLoadContextCommand(
            new Dictionary<string, List<string>> { { "loader", [] } }
        );

        var result = await _loadContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenNullFilesToLoad_ThenResultHasErrors()
    {
        var command = new WebLoadContextCommand(null!);

        var result = await _loadContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.FilesToLoad)
            .WithErrorMessage("'Files To Load' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyFilesToLoad_ThenResultHasNoErrors()
    {
        var command = new WebLoadContextCommand(new Dictionary<string, List<string>>());

        var result = await _loadContextValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }
}
