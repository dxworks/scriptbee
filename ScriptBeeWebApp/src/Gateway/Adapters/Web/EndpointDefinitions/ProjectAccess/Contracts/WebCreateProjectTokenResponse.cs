using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

public record WebCreateProjectTokenResponse(
    string Id,
    string Token,
    string? Description,
    string Role,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt
)
{
    public static WebCreateProjectTokenResponse Map(NewProjectTokenResult result)
    {
        return new WebCreateProjectTokenResponse(
            result.Token.Id.Value,
            result.UnhashedToken,
            result.Token.Description,
            result.Token.Role.Value,
            result.Token.CreatedAt,
            result.Token.ExpiresAt
        );
    }
}
