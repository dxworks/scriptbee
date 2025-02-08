namespace ScriptBee.Gateway.Web.Config;

public class JwtSettings
{
    public string Authority { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string AuthorizationUrl { get; set; } = null!;
    public string TokenUrl { get; set; } = null!;
    public string Realm { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
}
