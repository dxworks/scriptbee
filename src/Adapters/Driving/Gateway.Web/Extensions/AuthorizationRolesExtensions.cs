using Microsoft.AspNetCore.Authorization;
using ScriptBee.Domain.Model.Authorization;

namespace ScriptBee.Gateway.Web.Extensions;

public static class AuthorizationRolesExtensions
{
    public const string CreateProjectPolicy = "create_project";

    public static IServiceCollection AddAuthorizationRules(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .AddPolicyForCreateProject();
        return services;
    }

    private static AuthorizationBuilder AddPolicyForCreateProject(this AuthorizationBuilder authorizationBuilder)
    {
        return authorizationBuilder.AddPolicy(CreateProjectPolicy,
            policy => policy.RequireRole(UserRole.Administrator.Type));
    }
}
