using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Validation;

public class CreateProjectTokenValidator : AbstractValidator<WebCreateProjectTokenRequest>
{
    public CreateProjectTokenValidator()
    {
        RuleFor(x => x.ExpiresAt).NotNull();
        RuleFor(x => x.Role).NotEmpty();
    }
}
