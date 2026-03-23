using ScriptBee.Ports.Files;
using ScriptBee.Ports.Files.Config;

namespace ScriptBee.Web.Extensions;

public static class FileConfigExtensions
{
    public static IServiceCollection AddFileConfig(
        this IServiceCollection services,
        IConfigurationSection userFolderConfigurationSection
    )
    {
        services.Configure<UserFolderSettings>(userFolderConfigurationSection);
        return services
            .AddSingleton<IConfigFoldersService, ConfigFoldersService>()
            .AddSingleton<ICreateFile, CreateFileAdapter>();
    }
}
