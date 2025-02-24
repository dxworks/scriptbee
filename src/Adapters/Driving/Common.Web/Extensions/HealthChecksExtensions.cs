using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Common.Web.Extensions;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddConfiguredHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddMongoDb();
        return services;
    }

    public static void MapHealthChecksEndpoint(this WebApplication app)
    {
        app.MapHealthChecks(
            "/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponseNoExceptionDetails,
            }
        );
    }
}
