using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Project.Validation;

public class CreateProjectValidator : AbstractValidator<WebCreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
