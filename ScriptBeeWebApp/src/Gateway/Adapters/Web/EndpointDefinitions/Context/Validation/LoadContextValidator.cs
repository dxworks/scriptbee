using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Context.Validation;

public class LoadContextValidator : AbstractValidator<WebLoadContextCommand>
{
    public LoadContextValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                (x.LoaderIds != null && x.LoaderIds.Any())
                || (x.FilesToLoad != null && x.FilesToLoad.Count != 0)
            )
            .WithName("LoaderIds")
            .WithMessage("Either 'LoaderIds' or 'FilesToLoad' must be provided and non-empty.");
    }
}
