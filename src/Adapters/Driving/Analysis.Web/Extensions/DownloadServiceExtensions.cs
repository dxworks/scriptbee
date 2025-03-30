using ScriptBee.Persistence.File.Plugin.Installer;

namespace ScriptBee.Analysis.Web.Extensions;

public static class DownloadServiceExtensions
{
    public static IServiceCollection AddDownloadService(this IServiceCollection services)
    {
        // TODO FIXIT(#51): relocate in correct adapter and configure correctly
        services
            .AddSingleton<IDownloadService, DownloadService>()
            .AddHttpClient<DownloadService>();
        return services;
    }
}
