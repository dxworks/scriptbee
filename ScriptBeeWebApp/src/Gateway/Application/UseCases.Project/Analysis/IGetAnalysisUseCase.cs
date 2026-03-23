using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Project.Analysis;

public interface IGetAnalysisUseCase
{
    Task<IEnumerable<AnalysisInfo>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );

    Task<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );
}
