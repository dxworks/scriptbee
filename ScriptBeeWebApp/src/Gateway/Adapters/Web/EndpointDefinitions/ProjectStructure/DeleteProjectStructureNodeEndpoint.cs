using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class DeleteProjectStructureNodeEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IDeleteProjectFilesUseCase, DeleteProjectFilesService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/projects/{projectId}/files/{fileId}", DeleteProjectStructureNode)
            .WithTags("ProjectStructure");
    }

    private static async Task<
        Results<NoContent, NotFound<ProblemDetails>>
    > DeleteProjectStructureNode(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string fileId,
        IDeleteProjectFilesUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.Delete(
            new DeleteFileCommand(ProjectId.FromValue(projectId), new ScriptId(fileId)),
            cancellationToken
        );

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context)
        );
    }
}
