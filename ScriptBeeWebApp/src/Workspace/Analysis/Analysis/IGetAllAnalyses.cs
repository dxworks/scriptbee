using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Analysis;

public interface IGetAllAnalyses
{
    Task<IEnumerable<AnalysisInfo>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
