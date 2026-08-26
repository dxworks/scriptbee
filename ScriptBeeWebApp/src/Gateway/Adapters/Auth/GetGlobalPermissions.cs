using System.Net.Http.Json;
using ScriptBee.Adapters.Auth.Contracts;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Adapters.Auth;

public sealed class GetGlobalPermissions(IHttpClientFactory httpClientFactory)
    : IGetGlobalPermissions
{
    public const string ClientName = "GetGlobalPermissions";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(ClientName);

    public async Task<List<string>> GetPermissions(
        UserId userId,
        List<UserGroup> groups,
        CancellationToken cancellationToken
    )
    {
        var request = GetGlobalRequest(userId, groups);
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

    private static PermissionsRequest GetGlobalRequest(UserId userId, List<UserGroup> groups)
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
                Resource = new ExternalAuthorizationResource { Type = "global" },
            },
        };
    }
}
