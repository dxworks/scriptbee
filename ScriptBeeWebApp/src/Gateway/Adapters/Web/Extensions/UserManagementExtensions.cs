using ScriptBee.Service.Gateway;
using ScriptBee.Service.Gateway.Config;
using ScriptBee.UseCases.Gateway;

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

        return services.AddMemoryCache().AddSingleton<IManageUsersUseCase, ManageUsersService>();
    }
}
