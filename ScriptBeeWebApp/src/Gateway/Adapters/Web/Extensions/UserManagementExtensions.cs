using ScriptBee.Service.Gateway.Config;

namespace ScriptBee.Web.Extensions;

public static class UserManagementExtensions
{
    public static IServiceCollection AddUserManagementServices(
        this IServiceCollection services,
        string userManagementConfiguration
    )
    {
        services
            .AddOptions<ScriptBeeUserManagementConfig>()
            .BindConfiguration(userManagementConfiguration);

        return services.AddMemoryCache();
    }
}
