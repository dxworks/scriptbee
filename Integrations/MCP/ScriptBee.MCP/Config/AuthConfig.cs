namespace ScriptBee.MCP.Config;

public sealed class AuthConfig
{
    public const string SectionName = "Authentication";

    public string? Authority { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Scope { get; set; }
    public string? AccessToken { get; set; }
    public string? TokenEndpoint { get; set; }
    public bool RequireHttpsMetadata { get; set; } = true;
}
