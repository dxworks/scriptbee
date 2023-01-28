using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class CreateProjectValidatorTests
{
    private readonly CreateProjectValidator _createProjectValidator;

    public CreateProjectValidatorTests()
    {
        _createProjectValidator = new CreateProjectValidator();
    }

    [Fact]
    public async Task GivenValidCreateProject_WhenValidate_ThenResultHasNoErrors()
    {
        var createProject = new CreateProject("id", "name");

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var createProject = new CreateProject("", "name");

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task GivenEmptyProjectName_WhenValidate_ThenResultHasErrors()
    {
        var createProject = new CreateProject("id", "");

        var result = await _createProjectValidator.TestValidateAsync(createProject);

        result.ShouldHaveValidationErrorFor(x => x.ProjectName);
    }
}
