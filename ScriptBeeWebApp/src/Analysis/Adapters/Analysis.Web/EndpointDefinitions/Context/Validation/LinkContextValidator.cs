using FluentValidation;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context.Validation;

public class LinkContextValidator : AbstractValidator<WebLinkContextCommand>
{
    public LinkContextValidator()
    {
        RuleFor(x => x.LinkerIds).NotNull();
    }
}
