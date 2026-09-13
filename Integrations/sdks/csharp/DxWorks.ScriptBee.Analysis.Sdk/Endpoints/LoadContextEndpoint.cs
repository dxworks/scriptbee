using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class LoadContextEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/load", LoadContext)
            .WithTags("Context")
            .WithSummary("Load data into analysis context")
            .WithDescription(
                "Loads data from files into the current analysis context using the provided loaders."
            )
            .WithRequestValidation<WebLoadContextCommand>()
            .WithName("Load");
    }

    private static async Task<NoContent> LoadContext(
        [FromBody] WebLoadContextCommand command,
        ILoadContextUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        await useCase.Load(ConvertFilesToLoad(command), cancellationToken);

        return TypedResults.NoContent();
    }

    private static Dictionary<string, IEnumerable<FileId>> ConvertFilesToLoad(
        WebLoadContextCommand command
    )
    {
        return command.FilesToLoad.ToDictionary(
            x => x.Key,
            x => x.Value.Select(f => new FileId(f))
        );
    }
}
