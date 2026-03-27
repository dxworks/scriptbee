using FluentValidation;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Validation;

public class WebInstallPluginCommandValidator : AbstractValidator<WebInstallPluginCommand>
{
    public WebInstallPluginCommandValidator()
    {
        RuleFor(x => x.PluginId).NotEmpty();
        RuleFor(x => x.Version).NotEmpty();
    }
}
