using FluentValidation;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class ScriptParameterValidator : AbstractValidator<ScriptParameter>
{
    public ScriptParameterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.Type)
            .Must(type => type is DxWorks.ScriptBee.Plugin.Api.Model.ScriptParameter.TypeString or DxWorks.ScriptBee.Plugin.Api.Model.ScriptParameter.TypeInteger
                or DxWorks.ScriptBee.Plugin.Api.Model.ScriptParameter.TypeBoolean or DxWorks.ScriptBee.Plugin.Api.Model.ScriptParameter.TypeFloat);
    }
}
