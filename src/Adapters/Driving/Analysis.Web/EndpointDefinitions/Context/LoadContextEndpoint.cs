using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.File;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Context;

public class LoadContextEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ILoadContextUseCase, LoadContextService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/context/load", LoadContext)
            .WithRequestValidation<WebLoadContextCommand>();
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
