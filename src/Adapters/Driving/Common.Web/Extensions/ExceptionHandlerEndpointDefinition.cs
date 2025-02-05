using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace ScriptBee.Common.Web.Extensions;

public static class ExceptionHandlerEndpointDefinition
{
    public static void UseExceptionEndpoint(this WebApplication app)
    {
        app.Map("/error", HandleError);
        app.UseExceptionHandler("/error");
    }

    private static IResult HandleError(HttpContext context, ILogger logger)
    {
        var exception = GetException(context, logger);

        return exception is BadHttpRequestException ? HandleEmptyRequestBody(context) : HandleGenericError(context);
    }

    private static IResult HandleGenericError(HttpContext context)
    {
        var (instance, requestId, traceId) = ProblemDetailsExtensions.GetAdditionalProblemDetails(context);
        return Results.InternalServerError(new ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Detail = "Please contact support or try again later.",
            Instance = GetOriginalEndpoint(context) ?? instance,
            Extensions =
            {
                { "requestId", requestId },
                { "traceId", traceId }
            }
        });
    }

    private static BadRequest<ProblemDetails> HandleEmptyRequestBody(HttpContext context)
    {
        var (instance, requestId, traceId) = ProblemDetailsExtensions.GetAdditionalProblemDetails(context);
        return TypedResults.BadRequest(new ProblemDetails
        {
            Title = "Request body is required.",
            Detail = "The request body was missing or empty.",
            Instance = GetOriginalEndpoint(context) ?? instance,
            Extensions =
            {
                { "requestId", requestId },
                { "traceId", traceId }
            }
        });
    }

    private static Exception? GetException(HttpContext context, ILogger logger)
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception?.InnerException is not null)
        {
            logger.Error(exception.InnerException, "Exception occurred because of {ExceptionMessage}",
                exception.Message);
        }
        else
        {
            logger.Error(exception, "Exception occurred");
        }

        return exception;
    }

    private static string? GetOriginalEndpoint(HttpContext context)
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        return exceptionHandlerPathFeature?.Path;
    }
}
