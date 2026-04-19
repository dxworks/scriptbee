using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway.Context;

public record GetLinkersQuery(ProjectId ProjectId, InstanceId InstanceId);
