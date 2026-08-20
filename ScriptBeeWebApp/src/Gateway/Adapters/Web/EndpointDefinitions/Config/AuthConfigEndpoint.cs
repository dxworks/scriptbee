using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using ScriptBee.Common.Web;
using ScriptBee.Web.Config;
using ScriptBee.Web.EndpointDefinitions.Config.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Config;

public class AuthConfigEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/config/auth", GetAuthConfig)
            .WithTags("Config")
            .WithName("GetAuthConfig")
            .WithSummary("Get authentication configuration")
            .WithDescription("Retrieves the authentication configuration for the application.");
    }

    private static Ok<WebAuthConfig> GetAuthConfig(
        HttpContext context,
        IOptions<AuthenticationConfig> authConfigOptions
    )
    {
        var config = authConfigOptions.Value;
        return TypedResults.Ok(
            new WebAuthConfig
            {
                AuthMode = config.AuthMode,
                Authority = config.Authority,
                AuthWellknownEndpointUrl = config.AuthWellknownEndpointUrl,
                ClientId = config.ClientId,
                Scope = config.Scope,
            }
        );
    }
}
