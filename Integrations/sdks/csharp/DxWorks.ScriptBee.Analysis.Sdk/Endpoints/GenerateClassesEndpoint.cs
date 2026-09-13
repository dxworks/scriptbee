using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Common.Web;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class GenerateClassesEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<FileBundler>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/generate-classes", GenerateClasses)
            .WithTags("Context")
            .WithSummary("Generate classes for analysis context")
            .WithDescription(
                "Generates script classes based on the current data context and returns them as a stream."
            )
            .WithName("GenerateClasses");
    }

    private static async Task<Results<FileStreamHttpResult, ProblemHttpResult>> GenerateClasses(
        [FromBody] WebGenerateClassesRequest request,
        IGenerateClassesUseCase useCase,
        FileBundler fileBundler,
        CancellationToken cancellationToken
    )
    {
        var languages = request.Languages ?? [];
        var files = await useCase.GenerateClasses(languages, cancellationToken);

        var stream = new MemoryStream();
        await fileBundler.WriteToStream(files, stream, cancellationToken);
        stream.Position = 0;

        return TypedResults.Stream(stream, "application/octet-stream", "classes.bin");
    }
}
