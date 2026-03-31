using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;

namespace ScriptBee.UseCases.Project.Analysis;

public interface IGetAnalysisUseCase
{
    Task<IEnumerable<AnalysisInfo>> GetAll(
        GetAnalysisQuery query,
        CancellationToken cancellationToken = default
    );

    Task<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );
}
