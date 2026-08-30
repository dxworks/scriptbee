using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public record CreateProjectTokenCommand(
    ProjectId ProjectId,
    string? Description,
    UserRole Role,
    DateTimeOffset ExpiresAt
);
