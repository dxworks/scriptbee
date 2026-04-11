using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Common.Web;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context;

public class GenerateClassesEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGenerateClassesUseCase, GenerateClassesService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/generate-classes", GenerateClasses).WithTags("Context");
    }

    private static async Task<NoContent> GenerateClasses(
        IGenerateClassesUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        await useCase.GenerateClasses(cancellationToken);

        return TypedResults.NoContent();
    }
}
