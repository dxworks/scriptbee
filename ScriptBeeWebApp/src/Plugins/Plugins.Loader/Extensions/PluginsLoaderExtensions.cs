using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Plugins.Loader.Extensions;

public static class PluginsLoaderExtensions
{
    public static IServiceCollection AddPluginLoader(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPluginRepository, PluginRepository>()
            .AddSingleton<IDllLoader, DllLoader>()
            .AddSingleton<IPluginLoader, PluginLoader>()
            .AddSingleton<IPluginRegistrationService>(_ => RegisterPluginTypes());
    }

    private static PluginRegistrationService RegisterPluginTypes()
    {
        var pluginRegistrationService = new PluginRegistrationService();

        pluginRegistrationService.Add(pluginKind: PluginKind.Loader, [typeof(IModelLoader)]);
        pluginRegistrationService.Add(PluginKind.Linker, [typeof(IModelLinker)]);
        pluginRegistrationService.Add(
            PluginKind.ScriptGenerator,
            [typeof(IScriptGeneratorStrategy)]
        );
        pluginRegistrationService.Add(PluginKind.ScriptRunner, [typeof(IScriptRunner)]);
        pluginRegistrationService.Add(PluginKind.HelperFunctions, [typeof(IHelperFunctions)]);

        // todo: see how to start the plugin via node or http-server or something like that if not already started
        pluginRegistrationService.Add(PluginKind.Ui, []);

        return pluginRegistrationService;
    }
}
