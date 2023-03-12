using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class UpdateScriptValidator : AbstractValidator<UpdateScript>
{
    public UpdateScriptValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleForEach(x => x.Parameters)
            .SetValidator(new ScriptParameterValidator());
    }
}
