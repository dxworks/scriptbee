using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Validation;

public class CreateScriptValidator : AbstractValidator<WebCreateScriptCommand>
{
    private static readonly List<string> AllowedTypes = ["string", "integer", "float", "boolean"];

    public CreateScriptValidator()
    {
        RuleFor(x => x.Path).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
        RuleForEach(x => x.Parameters)
            .ChildRules(parameter =>
            {
                parameter.RuleFor(p => p.Name).NotEmpty();
                parameter.RuleFor(p => p.Type).NotEmpty();
                parameter
                    .RuleFor(p => p.Type)
                    .Must(x => AllowedTypes.Contains(x))
                    .WithMessage(
                        $"'{nameof(WebScriptParameter.Type)}' must be one of the following values: {string.Join(",", AllowedTypes)}."
                    );
            });
    }
}
