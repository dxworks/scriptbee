using Microsoft.AspNetCore.Diagnostics;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ILogger = Serilog.ILogger;

namespace ScriptBeeWebApp.EndpointDefinitions;

public class ExceptionHandlerEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.Map("/error", HandleError);
    }

    private static async Task HandleError(HttpContext context, ILogger logger)
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (exceptionHandlerPathFeature is null)
        {
            return;
        }

        var exception = exceptionHandlerPathFeature.Error;

        if (exception.InnerException is not null)
        {
            logger.Error(exception.InnerException, "Exception occurred because of {ExceptionMessage}",
                exception.Message);
        }
        else
        {
            logger.Error(exception, "Exception occurred");
        }

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new EndpointError(exception.Message));
    }
}
