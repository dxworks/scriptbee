using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Plugins.Installer.Extensions;

public static class PluginsInstallerExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPluginInstaller(string userFolderConfigurationSection)
        {
            // TODO FIXIT: this has to go where the configuration of the IFIlePahtProvider lies
            // services.AddOptions<PluginsSettings>().BindConfiguration(pluginConfigurationSection);

            return services.AddDownloadService().AddServices();
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
                .AddSingleton<IFileService, FileService>()
                .AddSingleton<IBundlePluginInstaller, BundlePluginInstaller>()
                .AddSingleton<ISimplePluginInstaller, SimplePluginInstaller>()
                .AddSingleton<IZipFileService, ZipFileService>()
                .AddSingleton<IDownloadService, DownloadService>()
                .AddSingleton<IPluginUninstaller, PluginUninstaller>()
                .AddSingleton<IBundlePluginUninstaller, BundlePluginUninstaller>();
        }
    }
}
