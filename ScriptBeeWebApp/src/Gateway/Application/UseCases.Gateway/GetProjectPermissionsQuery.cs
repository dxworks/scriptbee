using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public sealed record GetProjectPermissionsQuery(
    ProjectId ProjectId,
    UserId UserId,
    List<UserGroup> Groups
);
