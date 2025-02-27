using OneOf;
using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Project.Analysis;

public interface IGetAnalysisResult
{
    Task<OneOf<AnalysisResult, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );
}
