using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Common.Web.Extensions;

namespace DxWorks.ScriptBee.Analysis.Sdk.Extensions;

public static class TelemetrySdkExtensions
{
    public static IServiceCollection AddAnalysisSdkTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName = "scriptbee.analysis"
    )
    {
        return services.AddScriptBeeTelemetry(configuration, serviceName);
    }
}
