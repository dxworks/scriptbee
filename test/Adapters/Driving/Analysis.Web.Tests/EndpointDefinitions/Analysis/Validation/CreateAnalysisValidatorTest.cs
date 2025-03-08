﻿using FluentValidation.TestHelper;
using ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Contracts;
using ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Validation;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Analysis.Validation;

public class CreateAnalysisValidatorTest
{
    private readonly CreateAnalysisValidator _createAnalysisValidator = new();

    [Fact]
    public async Task GivenValidCreateProject_ThenResultHasNoErrors()
    {
        var command = new WebCreateAnalysisCommand("project-id", "script-id");

        var result = await _createAnalysisValidator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_ThenResultHasErrors()
    {
        var command = new WebCreateAnalysisCommand("", "script-id");

        var result = await _createAnalysisValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("'Project Id' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyScriptId_ThenResultHasErrors()
    {
        var command = new WebCreateAnalysisCommand("project-id", "");

        var result = await _createAnalysisValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.ScriptId)
            .WithErrorMessage("'Script Id' must not be empty.");
    }

    [Fact]
    public async Task GivenNullProjectId_ThenResultHasErrors()
    {
        var command = new WebCreateAnalysisCommand(null!, "script-id");

        var result = await _createAnalysisValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("'Project Id' must not be empty.");
    }

    [Fact]
    public async Task GivenNullScriptId_ThenResultHasErrors()
    {
        var command = new WebCreateAnalysisCommand("project-id", null!);

        var result = await _createAnalysisValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.ScriptId)
            .WithErrorMessage("'Script Id' must not be empty.");
    }

    [Fact]
    public async Task GivenInvalidFields_ThenResultHasErrors()
    {
        var command = new WebCreateAnalysisCommand(null!, null!);

        var result = await _createAnalysisValidator.TestValidateAsync(command);

        result
            .ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("'Project Id' must not be empty.");
        result
            .ShouldHaveValidationErrorFor(x => x.ScriptId)
            .WithErrorMessage("'Script Id' must not be empty.");
    }
}
