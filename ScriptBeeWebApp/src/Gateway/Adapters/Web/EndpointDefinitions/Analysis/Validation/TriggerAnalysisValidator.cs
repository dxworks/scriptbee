using FluentValidation;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Validation;

public class TriggerAnalysisValidator : AbstractValidator<WebTriggerAnalysisCommand>
{
    public TriggerAnalysisValidator()
    {
        RuleFor(x => x.ScriptId).NotEmpty();
    }
}
