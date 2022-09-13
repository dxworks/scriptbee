using FluentValidation;

namespace ScriptBeeWebApp.Controllers.Arguments.Validation;

public class LinkProjectValidator : AbstractValidator<LinkProject>
{
    public LinkProjectValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.LinkerName)
            .NotEmpty();
    }
}
