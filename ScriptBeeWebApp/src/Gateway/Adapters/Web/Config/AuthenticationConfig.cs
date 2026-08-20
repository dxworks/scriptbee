namespace ScriptBee.Web.Config;

public class AuthenticationConfig
{
    public string? AuthMode { get; init; }
    public required bool RequireHttpsMetadata { get; init; }
    public string? Authority { get; init; }
    public string? Audience { get; init; }
    public string? AuthWellknownEndpointUrl { get; init; }
    public string? ClientId { get; init; }
    public string? Scope { get; init; }
    public string? OpaUrl { get; init; }

    public bool IsDevelopment =>
        AuthMode?.Equals("Development", StringComparison.OrdinalIgnoreCase) ?? false;
}
