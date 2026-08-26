using FluentValidation;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebUpdateProjectMemberCommand(string Role, string MemberType);

public class WebUpdateProjectMemberCommandValidator
    : AbstractValidator<WebUpdateProjectMemberCommand>
{
    public WebUpdateProjectMemberCommandValidator()
    {
        RuleFor(x => x.Role).NotEmpty();
        RuleFor(x => x.MemberType)
            .NotEmpty()
            .Must(t => t == "user" || t == "group")
            .WithMessage("MemberType must be 'user' or 'group'.");
    }
}
