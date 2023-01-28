using Microsoft.AspNetCore.Diagnostics;

namespace ScriptBeeWebApp.EndpointDefinitions;

public class ExceptionHandlerEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/error", HandleError);
    }

    private async Task HandleError(HttpContext context, Serilog.ILogger logger)
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
        
        await context.Response.WriteAsJsonAsync(new EndpointError(exception.Message));
    }
}

public record EndpointError(string Error);
