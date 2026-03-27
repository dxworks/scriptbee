using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Artifacts.Extensions;
using ScriptBee.Persistence.File.Plugin;
using ScriptBee.Persistence.File.Plugin.Installer;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Plugins.Installer;

namespace ScriptBee.Persistence.File.Extensions;

public static class ConfigureFileAdapterExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddFileAdapters(
            IConfigurationSection userFolderConfigurationSection
        )
        {
            return services
                .AddArtifactFileAdapters(userFolderConfigurationSection)
                .AddPluginInstallerServices()
                .AddSingleton<IPluginDiscriminatorHolder, PluginDiscriminatorHolder>()
                .AddSingleton<IPluginManifestYamlFileReader, PluginManifestYamlFileReader>()
                .AddSingleton<IPluginReader, PluginReader>();
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
}
