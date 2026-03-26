using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Artifacts.Extensions;
using ScriptBee.Persistence.File.Plugin;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Persistence.File.Extensions;

public static class ConfigureFileAdapterExtensions
{
    public static IServiceCollection AddFileAdapters(
        this IServiceCollection services,
        IConfigurationSection userFolderConfigurationSection
    )
    {
        return services
            .AddArtifactFileAdapters(userFolderConfigurationSection)
            .AddSingleton<IPluginDiscriminatorHolder, PluginDiscriminatorHolder>()
            .AddSingleton<IPluginManifestYamlFileReader, PluginManifestYamlFileReader>()
            .AddSingleton<IPluginReader, PluginReader>();
    }
}
