using FluentValidation.TestHelper;
using ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Contracts;
using ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Validation;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Analysis.Validation;

public class RunAnalysisValidatorTest
{
    private readonly RunAnalysisValidator _runAnalysisValidator = new();

    [Fact]
    public async Task GivenValidCreateProject_ThenResultHasNoErrors()
    {
        var command = new WebRunAnalysisCommand("project-id", "script-id");

        var result = await _runAnalysisValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_ThenResultHasErrors()
    {
        var command = new WebRunAnalysisCommand("", "script-id");

        var result = await _runAnalysisValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("'Project Id' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyScriptId_ThenResultHasErrors()
    {
        var command = new WebRunAnalysisCommand("project-id", "");

        var result = await _runAnalysisValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.ScriptId)
            .WithErrorMessage("'Script Id' must not be empty.");
    }

    [Fact]
    public async Task GivenNullProjectId_ThenResultHasErrors()
    {
        var command = new WebRunAnalysisCommand(null!, "script-id");

        var result = await _runAnalysisValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("'Project Id' must not be empty.");
    }

    [Fact]
    public async Task GivenNullScriptId_ThenResultHasErrors()
    {
        var command = new WebRunAnalysisCommand("project-id", null!);

        var result = await _runAnalysisValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.ScriptId)
            .WithErrorMessage("'Script Id' must not be empty.");
    }

    [Fact]
    public async Task GivenInvalidFields_ThenResultHasErrors()
    {
        var command = new WebRunAnalysisCommand(null!, null!);

        var result = await _runAnalysisValidator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("'Project Id' must not be empty.");
        result
            .ShouldHaveValidationErrorFor(x => x.ScriptId)
            .WithErrorMessage("'Script Id' must not be empty.");
    }
}
