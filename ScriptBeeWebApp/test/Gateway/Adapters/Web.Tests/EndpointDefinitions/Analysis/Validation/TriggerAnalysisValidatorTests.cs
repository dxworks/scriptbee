using FluentValidation.TestHelper;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;
using ScriptBee.Web.EndpointDefinitions.Analysis.Validation;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Analysis.Validation;

public class TriggerAnalysisValidatorTests
{
    private readonly TriggerAnalysisValidator _validator = new();

    [Fact]
    public async Task GivenValidCreateProject_ThenResultHasNoErrors()
    {
        var createProject = new WebTriggerAnalysisCommand("id");

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyId_ThenResultHasErrors()
    {
        var createProject = new WebTriggerAnalysisCommand("");

        var result = await _validator.TestValidateAsync(
            createProject,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.ScriptId)
            .WithErrorMessage("'Script Id' must not be empty.");
    }
}
