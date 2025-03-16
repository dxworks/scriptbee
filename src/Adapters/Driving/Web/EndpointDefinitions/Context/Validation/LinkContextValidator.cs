using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Context.Validation;

public class LinkContextValidator : AbstractValidator<WebLinkContextCommand>
{
    public LinkContextValidator()
    {
        RuleFor(x => x.LinkerIds).NotEmpty();
    }
}
