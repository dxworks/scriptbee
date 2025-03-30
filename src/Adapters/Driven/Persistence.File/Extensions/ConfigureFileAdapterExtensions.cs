using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Persistence.File.Config;
using ScriptBee.Persistence.File.Plugin;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Persistence.File.Extensions;

public static class ConfigureFileAdapterExtensions
{
    public static IServiceCollection AddFileAdapters(
        this IServiceCollection services,
        IConfigurationSection userFolderConfigurationSection
    )
    {
        services.Configure<UserFolderSettings>(userFolderConfigurationSection);
        return services
            .AddSingleton<IConfigFoldersService, ConfigFoldersService>()
            .AddSingleton<ICreateFile, CreateFileAdapter>()
            .AddSingleton<ILoadFile, LoadFileAdapter>()
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IPluginDiscriminatorHolder, PluginDiscriminatorHolder>()
            .AddSingleton<IPluginManifestYamlFileReader, PluginManifestYamlFileReader>()
            .AddSingleton<IPluginReader, PluginReader>();
    }
}
