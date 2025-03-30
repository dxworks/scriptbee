using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.Ports.Analysis;

public interface IGetAnalysis
{
    Task<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );
}
