using ScriptBee.Domain.Model.User;

namespace ScriptBee.Domain.Model.Project;

public record ProjectToken(
    ProjectTokenId Id,
    ProjectId ProjectId,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    UserRole Role,
    string TokenHash
);
