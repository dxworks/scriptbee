using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ScriptBee.Common.Web.Extensions;

public static class TelemetryExtensions
{
    public static IServiceCollection AddScriptBeeTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName
    )
    {
        var options =
            configuration.GetSection("ScriptBee:Telemetry").Get<TelemetryOptions>()
            ?? new TelemetryOptions();

        if (!options.Enabled)
        {
            return services;
        }

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);

        services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
                {
                    tracing.AddOtlpExporter(o => o.Endpoint = new Uri(options.OtlpEndpoint));
                }

                if (options.ConsoleExporterEnabled)
                {
                    tracing.AddConsoleExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
                {
                    metrics.AddOtlpExporter(o => o.Endpoint = new Uri(options.OtlpEndpoint));
                }

                if (options.ConsoleExporterEnabled)
                {
                    metrics.AddConsoleExporter();
                }
            });

        return services;
    }
}
