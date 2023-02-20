using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class CreateScriptValidator : AbstractValidator<CreateScript>
{
    public CreateScriptValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.FilePath)
            .NotEmpty();
        RuleFor(x => x.ScriptLanguage)
            .NotEmpty();
        RuleForEach(x => x.Parameters)
            .SetValidator(new ScriptParameterValidator());
        // TODO: validate for existing project   
    }
}
