using FluentValidation;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project.Validation;

public class CreateProjectValidator : AbstractValidator<WebCreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
