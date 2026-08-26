namespace ScriptBee.MCP.Auth;

public interface IOidcTokenService
{
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken);
}
