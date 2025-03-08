using FluentValidation;
using ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Contracts;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Validation;

public class CreateAnalysisValidator : AbstractValidator<WebCreateAnalysisCommand>
{
    public CreateAnalysisValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ScriptId).NotEmpty();
    }
}
