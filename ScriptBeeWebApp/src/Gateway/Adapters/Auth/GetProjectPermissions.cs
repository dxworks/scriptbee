using System.Net.Http.Json;
using ScriptBee.Adapters.Auth.Contracts;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Adapters.Auth;

public sealed class GetProjectPermissions(IHttpClientFactory httpClientFactory)
    : IGetProjectPermissions
{
    public const string ClientName = "GetProjectPermissions";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(ClientName);

    public async Task<List<string>> GetPermissions(
        ProjectId projectId,
        UserId userId,
        List<UserGroup> groups,
        UserRole userRole,
        CancellationToken cancellationToken
    )
    {
        var request = GetProjectRequest(userId, groups, projectId, userRole);
        var response = await _httpClient.PostAsJsonAsync(
            "",
            request,
            cancellationToken: cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<PermissionsResponse>(
            cancellationToken: cancellationToken
        );

        return result?.Permissions ?? [];
    }

    private static PermissionsRequest GetProjectRequest(
        UserId userId,
        List<UserGroup> groups,
        ProjectId projectId,
        UserRole? role
    )
    {
        return new PermissionsRequest
        {
            Input = new PermissionsRequestInput
            {
                Subject = new ExternalAuthorizationRequestSubject
                {
                    UserId = userId.Value,
                    Groups = [.. groups.Select(g => g.Value)],
                },
                Resource = new ExternalAuthorizationResource
                {
                    Type = "project",
                    Id = projectId.Value,
                    Role = role?.Value,
                },
            },
        };
    }
}
