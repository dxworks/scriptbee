using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public sealed record GetGlobalPermissionsQuery(UserId UserId, List<UserGroup> Groups);
