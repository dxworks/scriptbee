using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using FluentValidation;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Validation;

public class RunAnalysisValidator : AbstractValidator<WebRunAnalysisCommand>
{
    public RunAnalysisValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ScriptId).NotEmpty();
    }
}
