using ScriptBee.Domain.Model.User;
using ScriptBee.Web.Auth.Contracts;

namespace ScriptBee.Web.Auth;

public sealed class GetDefaultCreatorRole(IHttpClientFactory httpClientFactory)
    : IGetDefaultCreatorRole
{
    public const string ClientName = "GetDefaultCreatorRole";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(ClientName);

    public async Task<UserRole> GetRole(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetFromJsonAsync<DefaultCreatorRoleResponse>(
            "",
            cancellationToken: cancellationToken
        );

        return new UserRole(response!.Result);
    }
}
