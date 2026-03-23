using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Context.Validation;

public class LoadContextValidator : AbstractValidator<WebLoadContextCommand>
{
    public LoadContextValidator()
    {
        RuleFor(x => x.LoaderIds).NotEmpty();
    }
}
