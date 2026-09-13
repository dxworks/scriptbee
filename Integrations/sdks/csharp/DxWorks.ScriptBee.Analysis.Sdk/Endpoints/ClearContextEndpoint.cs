using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class ClearContextEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/clear", ClearContext)
            .WithTags("Context")
            .WithSummary("Clear analysis context")
            .WithDescription("Clears all data from the current analysis context.")
            .WithName("Clear");
    }

    private static NoContent ClearContext(IClearContextUseCase useCase)
    {
        useCase.Clear();

        return TypedResults.NoContent();
    }
}
