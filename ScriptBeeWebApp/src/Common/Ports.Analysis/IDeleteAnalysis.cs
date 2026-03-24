using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Analysis;

public interface IDeleteAnalysis
{
    Task DeleteById(AnalysisId analysisId, CancellationToken cancellationToken = default);

    Task DeleteAllByProjectId(ProjectId projectId, CancellationToken cancellationToken = default);
}
