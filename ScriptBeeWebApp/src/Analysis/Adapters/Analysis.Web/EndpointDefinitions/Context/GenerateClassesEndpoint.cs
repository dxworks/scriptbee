using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context;

public class GenerateClassesEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGenerateClassesUseCase, GenerateClassesService>();
        services.AddSingleton<FileBundler>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/generate-classes", GenerateClasses)
            .WithTags("Context")
            .WithSummary("Generate classes for analysis context")
            .WithDescription(
                "Generates script classes based on the current data context and returns them as a stream."
            );
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
