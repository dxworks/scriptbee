using System.Net;
using ScriptBee.Web.Auth.Contracts;

namespace ScriptBee.Web.Auth;

public partial class AuthorizeExternally(
    IHttpClientFactory httpClientFactory,
    ILogger<ExternalAuthorizationActionAuthorizationHandler> logger
) : IAuthorizeExternally
{
    public const string ClientName = "ExternalAuthorizationClient";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(ClientName);

    public async Task<bool> IsAllowed(
        ExternalAuthorizationRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _httpClient.PostAsJsonAsync(
            "",
            request,
            cancellationToken: cancellationToken
        );

        LogExternalAuthorizationServiceResponseStatusCode(
            response.StatusCode,
            request.Input.Action
        );

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<ExternalAuthorizationResponse>(
            cancellationToken: cancellationToken
        );

        return result is { Result: true };
    }

    [LoggerMessage(
        LogLevel.Debug,
        "External authorization service response: {StatusCode} for action {Action}"
    )]
    private partial void LogExternalAuthorizationServiceResponseStatusCode(
        HttpStatusCode statusCode,
        string action
    );
}
