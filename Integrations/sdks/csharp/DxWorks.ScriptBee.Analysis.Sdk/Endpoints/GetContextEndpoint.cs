using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class GetContextEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/context", GetContext)
            .WithTags("Context")
            .WithSummary("Get analysis context")
            .WithDescription("Retrieves the current data context from the analysis service.")
            .WithName("Context");
    }

    private static Ok<WebGetContextResponse> GetContext(IGetContextUseCase useCase)
    {
        var contextSlices = useCase.Get();

        return TypedResults.Ok(
            new WebGetContextResponse(contextSlices.Select(WebContextSlice.Map))
        );
    }
}
