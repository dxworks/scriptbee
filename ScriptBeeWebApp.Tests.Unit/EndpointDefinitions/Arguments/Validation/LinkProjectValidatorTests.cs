using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.EndpointDefinitions.Arguments.Validation;

public class LinkProjectTests
{
    private readonly LinkProjectValidator _validator;

    public LinkProjectTests()
    {
        _validator = new LinkProjectValidator();
    }

    [Fact]
    public async Task GivenValidLinkProject_WhenValidate_ThenResultHasNoErrors()
    {
        var linkProject = new LinkProject("id1", "name");

        var result = await _validator.TestValidateAsync(linkProject);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyProjectId_WhenValidate_ThenResultHasErrors()
    {
        var linkProject = new LinkProject("", "name");

        var result = await _validator.TestValidateAsync(linkProject);

        result.ShouldHaveValidationErrorFor(r => r.ProjectId);
    }

    [Fact]
    public async Task GivenEmpty_WhenValidate_ThenResultHasErrors()
    {
        var linkProject = new LinkProject("projectId", "");

        var result = await _validator.TestValidateAsync(linkProject);

        result.ShouldHaveValidationErrorFor(r => r.LinkerName);
    }
}
