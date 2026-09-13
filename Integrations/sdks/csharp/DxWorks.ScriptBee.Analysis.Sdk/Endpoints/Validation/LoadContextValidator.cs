using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using FluentValidation;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Validation;

public class LoadContextValidator : AbstractValidator<WebLoadContextCommand>
{
    public LoadContextValidator()
    {
        RuleFor(x => x.FilesToLoad).NotNull();
    }
}
