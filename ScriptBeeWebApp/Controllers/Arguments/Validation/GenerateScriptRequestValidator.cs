using FluentValidation;

namespace ScriptBeeWebApp.Controllers.Arguments.Validation;

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
