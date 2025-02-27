using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Project.Analysis;

public interface IGetAllAnalysisResults
{
    Task<IEnumerable<AnalysisResult>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    );
}
