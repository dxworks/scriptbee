using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Common.Web.Extensions;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddProblemDetailsDefaults(this IServiceCollection services)
    {
        return services
            .AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    var (instance, requestId, traceId) = GetAdditionalProblemDetails(context.HttpContext);

                    context.ProblemDetails.Instance = instance;
                    context.ProblemDetails.Extensions.TryAdd("requestId", requestId);
                    context.ProblemDetails.Extensions.TryAdd("traceId", traceId);
                };
            });
    }
    
    public static (string instance, string? requestId, ActivityTraceId? traceId) GetAdditionalProblemDetails(
        HttpContext context)
    {
        var activity = context.Features.Get<IHttpActivityFeature>()?.Activity;

        var instance = context.Request.Path;
        var requestId = context.TraceIdentifier;
        var traceId = activity?.TraceId;

        return (instance, requestId, traceId);
    }
}
