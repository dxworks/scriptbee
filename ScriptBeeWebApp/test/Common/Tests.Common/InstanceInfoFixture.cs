using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Tests.Common;

public static class InstanceInfoFixture
{
    public static InstanceInfo BasicInstanceInfo(ProjectId projectId) =>
        new(new InstanceId(Guid.NewGuid()), projectId, "http://instance", DateTimeOffset.Now);
}
