using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Adapters.Auth.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Files;
using ScriptBee.UseCases.Gateway.Files;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Files;

using DeleteResult = Results<NoContent, NotFound<ProblemDetails>>;

public class DeleteSavedFileEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IDeleteSavedFileUseCase, DeleteSavedFileService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/projects/{projectId}/saved-files/{fileId}", DeleteSavedFile)
            .WithTags("Projects", "Files")
            .WithSummary("Delete a saved file")
            .WithDescription("Deletes a saved file from the project and cleans up references.")
            .RequireAction("model:upload");
    }

    private static async Task<DeleteResult> DeleteSavedFile(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string fileId,
        IDeleteSavedFileUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var command = new DeleteSavedFileCommand(
            ProjectId.FromValue(projectId),
            new FileId(fileId)
        );

        var result = await useCase.Delete(command, cancellationToken);

        return result.Match<DeleteResult>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }
}
