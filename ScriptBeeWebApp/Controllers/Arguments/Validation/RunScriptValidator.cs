using FluentValidation;

namespace ScriptBeeWebApp.Controllers.Arguments.Validation;

public class RunScriptValidator : AbstractValidator<RunScript>
{
    public RunScriptValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.FilePath)
            .NotEmpty();
        RuleFor(x => x.Language)
            .NotEmpty();
    }
}
