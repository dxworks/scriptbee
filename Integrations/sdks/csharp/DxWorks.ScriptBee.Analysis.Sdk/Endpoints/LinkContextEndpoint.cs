using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class LinkContextEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/link", LinkContext)
            .WithTags("Context")
            .WithSummary("Link analysis context")
            .WithDescription("Links the current analysis context using the provided linkers.")
            .WithRequestValidation<WebLinkContextCommand>()
            .WithName("Link");
    }

    private static async Task<NoContent> LinkContext(
        [FromBody] WebLinkContextCommand command,
        ILinkContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        await useCase.Link(command.LinkerIds, cancellationToken);

        return TypedResults.NoContent();
    }
}
