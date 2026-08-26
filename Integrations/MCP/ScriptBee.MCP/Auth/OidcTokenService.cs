using System.Text.Json;
using Microsoft.Extensions.Options;
using ScriptBee.MCP.Config;

namespace ScriptBee.MCP.Auth;

public sealed class OidcTokenService : IOidcTokenService, IDisposable
{
    private readonly AuthConfig _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OidcTokenService> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _cachedToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;
    private string? _discoveredTokenEndpoint;

    public OidcTokenService(
        IOptions<AuthConfig> authConfigOptions,
        HttpClient httpClient,
        ILogger<OidcTokenService> logger
    )
    {
        _config = authConfigOptions.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_config.AccessToken))
        {
            return _config.AccessToken;
        }

        if (
            string.IsNullOrWhiteSpace(_config.ClientId)
            || string.IsNullOrWhiteSpace(_config.ClientSecret)
            || string.IsNullOrWhiteSpace(_config.Authority)
        )
        {
            return null;
        }

        if (_cachedToken != null && DateTimeOffset.UtcNow < _tokenExpiry)
        {
            return _cachedToken;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_cachedToken != null && DateTimeOffset.UtcNow < _tokenExpiry)
            {
                return _cachedToken;
            }

            var tokenEndpoint = await ResolveTokenEndpointAsync(cancellationToken);
            var tokenResponse = await RequestClientCredentialsTokenAsync(
                tokenEndpoint,
                cancellationToken
            );

            _cachedToken = tokenResponse.AccessToken;
            var expiresInSeconds = tokenResponse.ExpiresIn > 0 ? tokenResponse.ExpiresIn : 3600;
            _tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(Math.Max(expiresInSeconds - 60, 30));

            return _cachedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to acquire OIDC token using client credentials.");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<string> ResolveTokenEndpointAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_discoveredTokenEndpoint))
        {
            return _discoveredTokenEndpoint;
        }

        if (!string.IsNullOrWhiteSpace(_config.TokenEndpoint))
        {
            _discoveredTokenEndpoint = _config.TokenEndpoint;
            return _discoveredTokenEndpoint;
        }

        var authority = _config.Authority!.TrimEnd('/');
        var discoveryUrl = $"{authority}/.well-known/openid-configuration";

        try
        {
            var discoveryDoc = await _httpClient.GetFromJsonAsync<JsonDocument>(
                discoveryUrl,
                cancellationToken
            );

            if (
                discoveryDoc != null
                && discoveryDoc.RootElement.TryGetProperty(
                    "token_endpoint",
                    out var tokenEndpointProp
                )
            )
            {
                var endpoint = tokenEndpointProp.GetString();
                if (!string.IsNullOrWhiteSpace(endpoint))
                {
                    _discoveredTokenEndpoint = endpoint;
                    return _discoveredTokenEndpoint;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to fetch OpenID discovery document from {DiscoveryUrl}, falling back to default token endpoint.",
                discoveryUrl
            );
        }

        _discoveredTokenEndpoint = $"{authority}/protocol/openid-connect/token";
        return _discoveredTokenEndpoint;
    }

    private async Task<OidcTokenResponse> RequestClientCredentialsTokenAsync(
        string tokenEndpoint,
        CancellationToken cancellationToken
    )
    {
        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _config.ClientId! },
            { "client_secret", _config.ClientSecret! },
        };

        if (!string.IsNullOrWhiteSpace(_config.Scope))
        {
            parameters.Add("scope", _config.Scope);
        }

        using var requestContent = new FormUrlEncodedContent(parameters);
        using var response = await _httpClient.PostAsync(
            tokenEndpoint,
            requestContent,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<OidcTokenResponse>(
            cancellationToken: cancellationToken
        );

        return tokenResponse
            ?? throw new InvalidOperationException(
                "Received empty token response from authorization server."
            );
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
