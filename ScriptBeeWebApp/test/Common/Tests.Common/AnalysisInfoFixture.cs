using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Tests.Common;

public static class AnalysisInfoFixture
{
    public static AnalysisInfo BasicAnalysisInfo(
        ProjectId projectId,
        AnalysisStatus? status = null
    ) =>
        new(
            new AnalysisId(Guid.NewGuid()),
            projectId,
            new InstanceId(Guid.NewGuid()),
            new ScriptId(Guid.NewGuid()),
            null,
            status ?? AnalysisStatus.Started,
            [],
            [],
            DateTimeOffset.UtcNow,
            null
        );
}
