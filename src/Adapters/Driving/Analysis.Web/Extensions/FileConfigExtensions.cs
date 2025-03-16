using ScriptBee.Persistence.File;
using ScriptBee.Persistence.File.Config;
using ScriptBee.Persistence.Mongodb;
using ScriptBee.Ports.Files;

namespace ScriptBee.Analysis.Web.Extensions;

public static class FileConfigExtensions
{
    public static IServiceCollection AddFileConfig(
        this IServiceCollection services,
        IConfigurationSection userFolderConfigurationSection
    )
    {
        services.Configure<UserFolderSettings>(userFolderConfigurationSection);
        return services
            .AddSingleton<IFileModelService, FileModelService>()
            .AddSingleton<IConfigFoldersService, ConfigFoldersService>()
            .AddSingleton<ICreateFile, CreateFileAdapter>()
            .AddSingleton<ILoadFile, LoadFileAdapter>();
    }
}
