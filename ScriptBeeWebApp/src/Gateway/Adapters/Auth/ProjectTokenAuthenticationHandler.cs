using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Adapters.Auth;

public class ProjectTokenAuthenticationHandler(
    IOptionsMonitor<ProjectTokenAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IValidateProjectTokenUseCase validateProjectTokenUseCase
) : AuthenticationHandler<ProjectTokenAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var rawToken = ExtractToken();

        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return AuthenticateResult.NoResult();
        }

        var token = await validateProjectTokenUseCase.ValidateToken(
            rawToken,
            Context.RequestAborted
        );
        if (token is null)
        {
            return AuthenticateResult.Fail("Invalid or expired project token.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, token.Id.Value),
            new Claim("token_id", token.Id.Value),
            new Claim("project_id", token.ProjectId.Value),
            new Claim("role", token.Role.Value),
            new Claim(ClaimTypes.Role, token.Role.Value),
            new Claim("token_type", "project_token"),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers.Append("WWW-Authenticate", "Bearer error=\"invalid_token\"");
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    private string? ExtractToken()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (
            !string.IsNullOrEmpty(authorization)
            && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
        )
        {
            return authorization["Bearer ".Length..].Trim();
        }

        if (
            Request.Path.StartsWithSegments("/api/projectLiveUpdates")
            && Request.Query.TryGetValue("access_token", out var tokenValue)
        )
        {
            return tokenValue.ToString();
        }

        return null;
    }
}
