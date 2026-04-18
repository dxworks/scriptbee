using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Plugins.Extensions;

public static class PluginsExtensions
{
    public static IServiceCollection AddPluginReader(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPluginDiscriminatorHolder, PluginDiscriminatorHolder>()
            .AddSingleton<IPluginManifestYamlFileReader, PluginManifestYamlFileReader>()
            .AddSingleton<IPluginReader, PluginReader>();
    }
}
