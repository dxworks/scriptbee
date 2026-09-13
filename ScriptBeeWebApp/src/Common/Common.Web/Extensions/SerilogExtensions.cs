using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ScriptBee.Common.Web.Extensions;

public static class SerilogExtensions
{
    public static IServiceCollection AddSerilog(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:o}][{Level:u3}][bootstrap] {Message:lj}{NewLine}{Exception}"
            )
            .CreateBootstrapLogger();

        return services;
    }

    public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog(
            (context, services, config) =>
                config.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services),
            preserveStaticLogger: false,
            writeToProviders: false
        );
    }
}
