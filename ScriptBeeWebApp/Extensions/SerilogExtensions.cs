using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace ScriptBeeWebApp.Extensions;

public static class SerilogExtensions
{
    public static IServiceCollection AddSerilog(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        Log.Logger = logger;

        services.AddSingleton<ILogger>(_ => logger);

        return services;
    }
}
