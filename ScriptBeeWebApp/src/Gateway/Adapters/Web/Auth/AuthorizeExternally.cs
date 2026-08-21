using System.Net;

namespace ScriptBee.Web.Auth;

public partial class AuthorizeExternally(
    IHttpClientFactory httpClientFactory,
    ILogger<ExternalAuthorizationActionAuthorizationHandler> logger
) : IAuthorizeExternally
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(
        "ExternalAuthorizationClient"
    );

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

        return result is { Allow: true };
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
