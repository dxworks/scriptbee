using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

public record WebProjectToken(
    string Id,
    string? Description,
    string Role,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt
)
{
    public static WebProjectToken Map(ProjectToken token)
    {
        return new WebProjectToken(
            token.Id.Value,
            token.Description,
            token.Role.Value,
            token.CreatedAt,
            token.ExpiresAt
        );
    }
}
