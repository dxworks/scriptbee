using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public sealed record CreateProjectCommand(string Id, string Name, UserId UserId);
