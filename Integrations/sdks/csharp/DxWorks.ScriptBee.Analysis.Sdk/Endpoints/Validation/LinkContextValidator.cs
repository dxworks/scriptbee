using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using FluentValidation;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Validation;

public class LinkContextValidator : AbstractValidator<WebLinkContextCommand>
{
    public LinkContextValidator()
    {
        RuleFor(x => x.LinkerIds).NotNull();
    }
}
