using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context;

public class GetContextEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetContextUseCase, GetContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/context", GetContext);
    }

    private static Ok<IEnumerable<WebContextSlice>> GetContext(IGetContextUseCase useCase)
    {
        var contextSlices = useCase.Get();

        return TypedResults.Ok(contextSlices.Select(WebContextSlice.Map));
    }
}
