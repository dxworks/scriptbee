using Microsoft.Extensions.DependencyInjection;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Extensions;

public static class FileWatcherExtensions
{
    public static IServiceCollection AddFileWatcherServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileWatcherHubService, FileWatcherHubService>();
        services.AddSingleton<IFileWatcherService, FileWatcherService>();
        return services;
    }
    
}
