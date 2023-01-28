using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class DownloadAllValidator : AbstractValidator<DownloadAll>
{
    public DownloadAllValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        RuleFor(x => x.RunIndex)
            .GreaterThanOrEqualTo(0);
    }
}
