using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Project.Analysis;

public interface IDeleteAnalysis
{
    Task DeleteById(AnalysisId analysisId, CancellationToken cancellationToken = default);

    Task DeleteAllByProjectId(ProjectId projectId, CancellationToken cancellationToken = default);
}
