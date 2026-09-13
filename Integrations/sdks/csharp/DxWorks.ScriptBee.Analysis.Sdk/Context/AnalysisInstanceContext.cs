using DxWorks.ScriptBee.Analysis.Sdk.Configuration;
using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace DxWorks.ScriptBee.Analysis.Sdk.Context;

public class AnalysisInstanceContext(IOptions<AnalysisInstanceOptions> options)
    : IAnalysisInstanceContext
{
    public InstanceId InstanceId { get; } =
        Guid.TryParse(options.Value.InstanceId, out var guid)
            ? new InstanceId(guid)
            : new InstanceId(Guid.Empty);

    public ProjectId ProjectId { get; } = ProjectId.FromValue(options.Value.ProjectId);

    public string ProjectName { get; } = options.Value.ProjectName;
}
