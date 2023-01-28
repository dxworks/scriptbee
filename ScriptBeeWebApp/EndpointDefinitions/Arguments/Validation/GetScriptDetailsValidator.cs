using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class GetScriptDetailsValidator : AbstractValidator<GetScriptDetails>
{
    public GetScriptDetailsValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.FilePath)
            .NotEmpty();
    }
}
