using FluentValidation.TestHelper;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Validation;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectAccess.Validation;

public class CreateProjectTokenValidatorTests
{
    private readonly CreateProjectTokenValidator _validator = new();

    [Fact]
    public async Task GivenValidCreateProjectToken_ThenResultHasNoErrors()
    {
        var request = new WebCreateProjectTokenRequest("CI token", "viewer", DateTimeOffset.UtcNow);

        var result = await _validator.TestValidateAsync(
            request,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenEmptyRole_ThenResultHasErrors()
    {
        var request = new WebCreateProjectTokenRequest("CI token", "", DateTimeOffset.UtcNow);

        var result = await _validator.TestValidateAsync(
            request,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("'Role' must not be empty.");
    }

    [Fact]
    public async Task GivenNullRole_ThenResultHasErrors()
    {
        var request = new WebCreateProjectTokenRequest("CI token", null!, DateTimeOffset.UtcNow);

        var result = await _validator.TestValidateAsync(
            request,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result
            .ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("'Role' must not be empty.");
    }

    [Fact]
    public async Task GivenDefaultExpiresAt_ThenResultHasNoErrors()
    {
        var request = new WebCreateProjectTokenRequest("CI token", "viewer", default);

        var result = await _validator.TestValidateAsync(
            request,
            cancellationToken: TestContext.Current.CancellationToken
        );

        result.ShouldNotHaveAnyValidationErrors();
    }
}
