using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Gateway.Web.Extensions;

public static class AuthorizationRolesExtensions
{
    public static IServiceCollection AddAuthorizationRules(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .AddPolicyForMaintainer("create_project");
        return services;
    }

    private static AuthorizationBuilder AddPolicyForMaintainer(this AuthorizationBuilder authorizationBuilder,
        string policyName)
    {
        return authorizationBuilder.AddPolicy(policyName, policy => policy.RequireRole("MAINTAINER"));
    }
}
