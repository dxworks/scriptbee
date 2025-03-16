using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context;

public class LinkContextEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ILinkContextUseCase, LinkContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/link", LinkContext)
            .WithRequestValidation<WebLinkContextCommand>();
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
