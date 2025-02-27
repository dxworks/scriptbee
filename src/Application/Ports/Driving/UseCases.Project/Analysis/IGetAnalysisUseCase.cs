using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

public interface IGetAnalysisUseCase
{
    Task<IEnumerable<AnalysisResult>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );

    Task<OneOf<AnalysisResult, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );
}
