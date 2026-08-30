using ScriptBee.Domain.Model.User;

namespace ScriptBee.Domain.Model.Project;

public record ProjectToken(
    ProjectTokenId Id,
    ProjectId ProjectId,
    string TokenHash,
    string? Description,
    UserRole Role,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt
);
