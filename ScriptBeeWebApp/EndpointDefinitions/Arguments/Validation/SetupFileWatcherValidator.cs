using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class SetupFileWatcherValidator : AbstractValidator<SetupFileWatcher>
{
    public SetupFileWatcherValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}
