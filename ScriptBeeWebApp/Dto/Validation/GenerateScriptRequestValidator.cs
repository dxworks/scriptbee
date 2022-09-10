using FluentValidation;

namespace ScriptBeeWebApp.Dto.Validation;

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
