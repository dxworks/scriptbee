using FluentValidation.TestHelper;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project.Validation;

namespace ScriptBee.Gateway.Web.Tests.EndpointDefinitions.Project.Validation;

public class CreateProjectValidatorTests
{
    private readonly CreateProjectValidator _createProjectValidator = new();

    [Fact]
    public async Task GivenValidCreateProject_ThenResultHasNoErrors()
    {
        var createProject = new WebCreateProjectCommand("id", "name");

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyId_ThenResultHasErrors()
    {
        var createProject = new WebCreateProjectCommand("", "name");

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("'Id' must not be empty.");
    }

    [Fact]
    public async Task GivenNullId_ThenResultHasErrors()
    {
        var createProject = new WebCreateProjectCommand(null!, "name");

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("'Id' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyName_ThenResultHasErrors()
    {
        var createProject = new WebCreateProjectCommand("id", "");

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("'Name' must not be empty.");
    }

    [Fact]
    public async Task GivenNullName_ThenResultHasErrors()
    {
        var createProject = new WebCreateProjectCommand("id", null!);

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("'Name' must not be empty.");
    }

    [Fact]
    public async Task GivenInvalidFields_ThenResultHasErrors()
    {
        var createProject = new WebCreateProjectCommand(null!, null!);

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("'Name' must not be empty.");
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("'Id' must not be empty.");
    }
}
