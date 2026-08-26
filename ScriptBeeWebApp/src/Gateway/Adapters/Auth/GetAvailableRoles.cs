using System.Net.Http.Json;
using ScriptBee.Adapters.Auth.Contracts;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Adapters.Auth;

public sealed class GetAvailableRoles(IHttpClientFactory httpClientFactory) : IGetAvailableRoles
{
    public const string ClientName = "GetRoles";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(ClientName);

    public async Task<List<RoleInfo>> GetRoles(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetFromJsonAsync<RolesResponse>(
            "",
            cancellationToken: cancellationToken
        );

        return response?.Roles.Select(r => new RoleInfo(r.Id, r.Description)).ToList() ?? [];
    }
}
