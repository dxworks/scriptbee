using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

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
