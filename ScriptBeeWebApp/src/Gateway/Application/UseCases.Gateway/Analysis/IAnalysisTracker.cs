using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Analysis;

public interface IAnalysisTracker
{
    void Track(AnalysisId analysisId, ProjectId projectId);
}
