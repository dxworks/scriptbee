using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Common.Web;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context;

public class ClearContextEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IClearContextUseCase, ClearContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/clear", ClearContext);
    }

    private static NoContent ClearContext(IClearContextUseCase useCase)
    {
        useCase.Clear();

        return TypedResults.NoContent();
    }
}
