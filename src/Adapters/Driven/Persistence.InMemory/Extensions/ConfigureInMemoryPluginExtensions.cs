using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Persistence.InMemory.Extensions;

public static class ConfigureInMemoryPluginExtensions
{
    public static IServiceCollection AddInMemoryPluginServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPluginRepository, PluginRepository>()
            .AddSingleton<IPluginRegistrationService>(_ => RegisterPluginTypes());
    }

    private static PluginRegistrationService RegisterPluginTypes()
    {
        var pluginRegistrationService = new PluginRegistrationService();

        pluginRegistrationService.Add(PluginKind.Loader, [typeof(IModelLoader)]);
        pluginRegistrationService.Add(PluginKind.Linker, [typeof(IModelLinker)]);
        pluginRegistrationService.Add(
            PluginKind.ScriptGenerator,
            [typeof(IScriptGeneratorStrategy)]
        );
        pluginRegistrationService.Add(PluginKind.ScriptRunner, [typeof(IScriptRunner)]);
        pluginRegistrationService.Add(PluginKind.HelperFunctions, [typeof(IHelperFunctions)]);

        // todo: see how to start the plugin via node or http-server or something like that if not already started
        pluginRegistrationService.Add(PluginKind.Ui, new HashSet<Type>());

        return pluginRegistrationService;
    }
}
