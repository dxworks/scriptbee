using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Tests.Common;

public static class InstanceInfoFixture
{
    public static InstanceInfo BasicInstanceInfo(ProjectId projectId) =>
        BasicInstanceInfo(new InstanceId(Guid.NewGuid()), projectId);

    public static InstanceInfo BasicInstanceInfo(InstanceId instanceId, ProjectId projectId) =>
        new(instanceId, projectId, "http://instance", DateTimeOffset.Now);
}
