using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Artifacts.Config;

namespace ScriptBee.Artifacts.Extensions;

public static class ConfigureProjectScriptingExtensions
{
    public static IServiceCollection AddArtifactFileAdapters(
        this IServiceCollection services,
        IConfigurationSection userFolderConfigurationSection
    )
    {
        services.Configure<UserFolderSettings>(userFolderConfigurationSection);
        return services
            .AddSingleton<IConfigFoldersService, ConfigFoldersService>()
            .AddSingleton<ICreateFile, CreateFileAdapter>()
            .AddSingleton<ILoadFile, LoadFileAdapter>()
            .AddSingleton<IUpdateFile, UpdateFileAdapter>()
            .AddSingleton<IFileService, FileService>();
    }
}
