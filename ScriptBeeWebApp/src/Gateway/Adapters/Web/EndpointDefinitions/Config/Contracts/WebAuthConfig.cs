namespace ScriptBee.Web.EndpointDefinitions.Config.Contracts;

public class WebAuthConfig
{
    public string? AuthMode { get; init; }
    public string? Authority { get; init; }
    public string? ClientId { get; init; }
    public string? Scope { get; init; }
}
