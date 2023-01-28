using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class GenerateScriptRequestValidator : AbstractValidator<GenerateScriptRequest>
{
    public GenerateScriptRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.ScriptType)
            .NotEmpty();
    }
}
