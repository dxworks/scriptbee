using FluentValidation.TestHelper;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Validation;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectAccess.Validation;

public class UpdateProjectMemberCommandValidatorTests
{
    private readonly UpdateProjectMemberCommandValidator _validator = new();

    [Fact]
    public async Task GivenValidUpdateProjectMember_ThenResultHasNoErrors()
    {
        var command = new WebUpdateProjectMemberCommand("editor", "user");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyRole_ThenResultHasErrors()
    {
        var command = new WebUpdateProjectMemberCommand("", "user");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("'Role' must not be empty.");
    }

    [Fact]
    public async Task GivenEmptyMemberType_ThenResultHasErrors()
    {
        var command = new WebUpdateProjectMemberCommand("editor", "");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.MemberType)
            .WithErrorMessage("'Member Type' must not be empty.");
    }

    [Fact]
    public async Task GivenInvalidMemberType_ThenResultHasErrors()
    {
        var command = new WebUpdateProjectMemberCommand("editor", "team");

        var result = await _validator.TestValidateAsync(
            command,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.MemberType)
            .WithErrorMessage("Member Type must be 'user' or 'group'.");
    }
}
