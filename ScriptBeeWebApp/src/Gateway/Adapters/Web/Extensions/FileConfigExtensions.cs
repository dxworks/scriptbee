using ScriptBee.Persistence.File;
using ScriptBee.Persistence.File.Config;
using ScriptBee.Ports.Files;

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
