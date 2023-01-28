using Microsoft.Extensions.DependencyInjection;
using ScriptBee.FileManagement;
using ScriptBee.Services;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Extensions;

public static class UtilityServicesExtensions
{
    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        services.AddSingleton<IGuidGenerator, GuidGenerator>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IDownloadService, DownloadService>();
        services.AddHttpClient<DownloadService>();
        services.AddSingleton<IZipFileService, ZipFileService>();
        return services;
    }
}
