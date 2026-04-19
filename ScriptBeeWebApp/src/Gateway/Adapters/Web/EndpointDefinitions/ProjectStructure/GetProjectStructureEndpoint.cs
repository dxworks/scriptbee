using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Gateway.ProjectStructure;
using ScriptBee.UseCases.Gateway.ProjectStructure;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class GetProjectStructureEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddScoped<IGetProjectFilesUseCase, GetProjectFilesService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/files", GetProjectFiles).WithTags("ProjectStructure");
        app.MapGet(
                "/api/projects/{projectId}/structure/available-script-types",
                GetAvailableScriptTypes
            )
            .WithTags("ProjectStructure");
    }

    private static async Task<
        Results<
            Ok<WebGetProjectFilesResponse>,
            BadRequest<ProblemDetails>,
            NotFound<ProblemDetails>
        >
    > GetProjectFiles(
        HttpContext context,
        IGetProjectFilesUseCase useCase,
        [FromRoute] string projectId,
        [FromQuery] string? parentId = null,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default
    )
    {
        if (offset < 0 || limit <= 0)
        {
            return ValidationApiErrorExtensions.ToInvalidPaginationProblem(context, offset, limit);
        }

        var result = await useCase.GetAll(
            new GetProjectFilesQuery(
                ProjectId.FromValue(projectId),
                parentId == null ? null : new ScriptId(parentId),
                offset,
                limit
            ),
            cancellationToken
        );

        return result.Match<
            Results<
                Ok<WebGetProjectFilesResponse>,
                BadRequest<ProblemDetails>,
                NotFound<ProblemDetails>
            >
        >(
            page =>
                TypedResults.Ok(
                    new WebGetProjectFilesResponse(
                        page.Data.Select(WebProjectFileNode.Map).ToList(),
                        page.TotalCount,
                        page.Offset,
                        page.Limit
                    )
                ),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }

    private static async Task<Ok<WebGetAvailableScriptTypesResponse>> GetAvailableScriptTypes(
        [FromRoute] string projectId
    )
    {
        await Task.CompletedTask;
        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok(
            new WebGetAvailableScriptTypesResponse([
                new WebScriptLanguage("csharp", ".cs"),
                new WebScriptLanguage("python", ".py"),
                new WebScriptLanguage("javascript", ".js"),
            ])
        );
    }
}
