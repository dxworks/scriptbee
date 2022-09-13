using FluentValidation;

namespace ScriptBeeWebApp.Controllers.Arguments.Validation;

// todo add tests
public class LoadModelsValidator : AbstractValidator<LoadModels>
{
    public LoadModelsValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.Nodes)
            .NotEmpty();
    }
}
