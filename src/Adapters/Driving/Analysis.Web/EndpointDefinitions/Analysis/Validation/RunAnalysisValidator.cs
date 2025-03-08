using FluentValidation;
using ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Contracts;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Validation;

public class RunAnalysisValidator : AbstractValidator<WebRunAnalysisCommand>
{
    public RunAnalysisValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ScriptId).NotEmpty();
    }
}
