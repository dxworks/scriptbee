using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Plugins.Extensions;

namespace ScriptBee.Plugins.Installer.Extensions;

public static class PluginsInstallerExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPluginInstaller()
        {
            return services.AddPluginReader().AddDownloadService().AddServices();
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
