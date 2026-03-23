using FluentValidation;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Validation;

public class LoadContextValidator : AbstractValidator<WebLoadContextCommand>
{
    public LoadContextValidator()
    {
        RuleFor(x => x.FilesToLoad).NotNull();
    }
}
