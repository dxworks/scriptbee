using FluentValidation;
using ScriptBee.Models;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;

public class ScriptParameterValidator : AbstractValidator<ScriptParameter>
{
    public ScriptParameterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.Type)
            .Must(type => type is ScriptParameterModel.TypeString or ScriptParameterModel.TypeInteger
                or ScriptParameterModel.TypeBoolean or ScriptParameterModel.TypeFloat);
    }
}
