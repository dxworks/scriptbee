using Microsoft.AspNetCore.Authorization;
using ScriptBee.Domain.Model.Authorization;
using ScriptBee.Gateway.Web.Config;

namespace ScriptBee.Gateway.Web.Extensions;

public static class AuthorizationRolesExtensions
{
    public const string CreateProjectPolicy = "create_project";
    public const string DeleteProjectPolicy = "delete_project";
    public const string ViewProjectPolicy = "view_project";

    public static TBuilder AddAuthorizationPolicy<TBuilder>(this TBuilder builder, string policyName,
        IConfiguration configuration) where TBuilder : IEndpointConventionBuilder
    {
        var featuresSettings = configuration.GetSection("Features").Get<FeaturesSettings>() ?? new FeaturesSettings();

        return featuresSettings.DisableAuthorization ? builder : builder.RequireAuthorization(policyName);
    }

    public static IServiceCollection AddAuthorizationRules(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .AddPoliciesForProject();
        return services;
    }

    private static AuthorizationBuilder AddPoliciesForProject(this AuthorizationBuilder authorizationBuilder)
    {
        return authorizationBuilder
            .AddPolicy(CreateProjectPolicy, policy => policy.RequireRole(UserRole.Administrator.Type))
            .AddPolicy(DeleteProjectPolicy, policy => policy.RequireRole(UserRole.Administrator.Type))
            .AddPolicy(ViewProjectPolicy,
                policy => policy.RequireRole(UserRole.Guest.Type,
                    UserRole.Analyst.Type, UserRole.Maintainer.Type, UserRole.Administrator.Type));
    }
}
