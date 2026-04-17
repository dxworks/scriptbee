using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Artifacts.Extensions;
using ScriptBee.Common.Plugins.Config;
using ScriptBee.Common.Plugins.Installer;
using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Common.Plugins.Extensions;

public static class PluginExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPlugins(
            string pluginConfigurationSection,
            string userFolderConfigurationSection
        )
        {
            services.AddOptions<PluginsSettings>().BindConfiguration(pluginConfigurationSection);

            return services
                .AddServices()
                .AddDownloadService()
                .AddArtifactFileAdapters(userFolderConfigurationSection)
                .AddPluginInstallerServices();
        }

        private IServiceCollection AddDownloadService()
        {
            services
                .AddSingleton<IDownloadService, DownloadService>()
                .AddHttpClient<DownloadService>();
            return services;
        }

        private IServiceCollection AddServices()
        {
            return services
                .AddSingleton<IPluginDiscriminatorHolder, PluginDiscriminatorHolder>()
                .AddSingleton<IPluginManifestYamlFileReader, PluginManifestYamlFileReader>()
                .AddSingleton<IPluginReader, PluginReader>()
                .AddSingleton<IPluginPathProvider, PluginPathProvider>()
                .AddSingleton<IPluginRepository, PluginRepository>()
                .AddSingleton<IPluginRegistrationService>(_ => RegisterPluginTypes());
        }

        private IServiceCollection AddPluginInstallerServices()
        {
            return services
                .AddSingleton<IBundlePluginInstaller, BundlePluginInstaller>()
                .AddSingleton<ISimplePluginInstaller, SimplePluginInstaller>()
                .AddSingleton<IZipFileService, ZipFileService>()
                .AddSingleton<IDownloadService, DownloadService>()
                .AddSingleton<IPluginUninstaller, PluginUninstaller>()
                .AddSingleton<IBundlePluginUninstaller, BundlePluginUninstaller>();
        }
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
