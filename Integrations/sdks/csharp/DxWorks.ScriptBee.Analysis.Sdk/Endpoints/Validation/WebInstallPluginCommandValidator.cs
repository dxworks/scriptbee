using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using FluentValidation;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Validation;

public class WebInstallPluginCommandValidator : AbstractValidator<WebInstallPluginCommand>
{
    public WebInstallPluginCommandValidator()
    {
        RuleFor(x => x.PluginId).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
    }
}
