using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Validation;

public class UpdateScriptValidator : AbstractValidator<WebUpdateScriptCommand>
{
    public UpdateScriptValidator()
    {
        RuleForEach(x => x.Parameters)
            .ChildRules(parameter =>
            {
                parameter.RuleFor(p => p.Name).NotEmpty();
                parameter.RuleFor(p => p.Type).NotEmpty();
                parameter
                    .RuleFor(p => p.Type)
                    .Must(x => AllowedScriptParameterTypes.AllowedTypes.Contains(x))
                    .WithMessage(
                        $"'{nameof(WebScriptParameter.Type)}' must be one of the following values: {string.Join(",", AllowedScriptParameterTypes.AllowedTypes)}."
                    );
            });
    }
}
