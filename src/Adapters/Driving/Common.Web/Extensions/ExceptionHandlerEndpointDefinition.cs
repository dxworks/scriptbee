using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        return Results.InternalServerError(context.ToProblemDetails(
            "An unexpected error occurred.",
            "Please contact support or try again later."
        ));
    }

    private static BadRequest<ProblemDetails> HandleEmptyRequestBody(HttpContext context)
    {
        return TypedResults.BadRequest(
            context.ToProblemDetails(
                "Request body is required.",
                "The request body was missing or empty."
            ));
    }

    private static Exception? GetException(HttpContext context, ILogger logger)
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception?.InnerException is not null)
        {
            logger.LogError(exception.InnerException, "Exception occurred because of {ExceptionMessage}",
                exception.Message);
        }
        else
        {
            logger.LogError(exception, "Exception occurred");
        }

        return exception;
    }
}
