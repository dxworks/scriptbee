using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class CreateProjectValidator : AbstractValidator<CreateProject>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.ProjectName)
            .NotEmpty();
        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}
