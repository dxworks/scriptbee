using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public sealed record GetAllProjectsQuery(UserId UserId, List<UserGroup> Groups);
