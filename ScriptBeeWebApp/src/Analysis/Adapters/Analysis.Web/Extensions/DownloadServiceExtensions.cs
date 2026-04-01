using ScriptBee.Persistence.File.Plugin.Installer;

namespace ScriptBee.Analysis.Web.Extensions;

public static class DownloadServiceExtensions
{
    public static IServiceCollection AddDownloadService(this IServiceCollection services)
    {
        services.AddSingleton<IDownloadService, DownloadService>().AddHttpClient<DownloadService>();
        return services;
    }
}
