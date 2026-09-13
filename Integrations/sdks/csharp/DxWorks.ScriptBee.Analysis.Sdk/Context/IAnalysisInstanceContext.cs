using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace DxWorks.ScriptBee.Analysis.Sdk.Context;

public interface IAnalysisInstanceContext
{
    InstanceId InstanceId { get; }
    ProjectId ProjectId { get; }
    string ProjectName { get; }
}
