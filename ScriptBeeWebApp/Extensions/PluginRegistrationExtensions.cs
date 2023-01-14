using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.FileManagement;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Installer;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Extensions;

public static class PluginRegistrationExtensions
{
    public static IServiceCollection AddPluginServices(this IServiceCollection services)
    {
        return services.AddSingleton<IPluginRepository, PluginRepository>()
            .AddSingleton<IPluginService, PluginService>()
            .AddSingleton<IPluginDiscriminatorHolder, PluginDiscriminatorHolder>()
            .AddSingleton<IPluginManifestYamlFileReader, PluginManifestYamlFileReader>()
            .AddSingleton<IPluginReader, PluginReader>()
            .AddSingleton<IDllLoader, DllLoader>()
            .AddSingleton<IPluginLoader, PluginLoader>()
            .AddSingleton<IPluginUrlFetcher, PluginUrlFetcher>()
            .AddSingleton<ISimplePluginInstaller, SimplePluginInstaller>()
            .AddSingleton<IPluginUninstaller, PluginUninstaller>()
            .AddSingleton<IBundlePluginInstaller, BundlePluginInstaller>()
            .AddSingleton<IBundlePluginUninstaller, BundlePluginUninstaller>()
            .AddSingleton<PluginManager>()
            .AddSingleton<IPluginRegistrationService>(_ => RegisterPluginTypes());
    }

    private static IPluginRegistrationService RegisterPluginTypes()
    {
        var pluginRegistrationService = new PluginRegistrationService();

        pluginRegistrationService.Add(PluginKind.Loader, new HashSet<Type> { typeof(IModelLoader) });
        pluginRegistrationService.Add(PluginKind.Linker, new HashSet<Type> { typeof(IModelLinker) });
        pluginRegistrationService.Add(PluginKind.ScriptGenerator,
            new HashSet<Type> { typeof(IScriptGeneratorStrategy) });
        pluginRegistrationService.Add(PluginKind.ScriptRunner, new HashSet<Type> { typeof(IScriptRunner) });
        pluginRegistrationService.Add(PluginKind.HelperFunctions,
            new HashSet<Type> { typeof(IHelperFunctions) });

        // todo see how to start the plugin via node or http-server or something like that if not already started
        pluginRegistrationService.Add(PluginKind.Ui, new HashSet<Type>());

        return pluginRegistrationService;
    }
}
