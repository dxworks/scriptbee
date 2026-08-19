using ScriptBee.Web.Config;

namespace ScriptBee.Web.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services)
    {
        services.AddOptions<AuthenticationConfig>().BindConfiguration("Authentication");

        return services;
    }
}
