using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Validation;

public class UpdateProjectMemberCommandValidator : AbstractValidator<WebUpdateProjectMemberCommand>
{
    public UpdateProjectMemberCommandValidator()
    {
        RuleFor(x => x.Role).NotEmpty();
        RuleFor(x => x.MemberType)
            .NotEmpty()
            .Must(t => t is "user" or "group")
            .WithMessage("Member Type must be 'user' or 'group'.");
    }
}
