using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
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
        app.MapGet("/api/projects/{projectId}/structure", GetProjectStructure)
            .WithTags("ProjectStructure");
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
        IGetScriptAbsolutePathUseCase absolutePathUseCase,
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
            queryResult =>
                TypedResults.Ok(
                    new WebGetProjectFilesResponse(
                        queryResult
                            .Data.Select(entry =>
                                WebProjectFileNode.Map(
                                    entry,
                                    absolutePathUseCase.GetScriptAbsolutePath(entry)
                                )
                            )
                            .ToList(),
                        queryResult.TotalCount,
                        queryResult.Offset,
                        queryResult.Limit
                    )
                ),
            error => error.ToProblem(context)
        );
    }

    private static async Task<Ok<WebGetProjectStructureResponse>> GetProjectStructure(
        [FromRoute] string projectId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok(
            new WebGetProjectStructureResponse([
                new WebProjectStructureNode
                {
                    Id = "folder-1",
                    Name = "folder-1",
                    Path = "folder-1",
                    AbsolutePath = $"{projectId}/folder-1",
                    Children =
                    [
                        new WebProjectStructureNode
                        {
                            Id = "sub-folder-1",
                            Name = "sub-folder-1",
                            Path = "folder-1/sub-folder-1",
                            AbsolutePath = $"{projectId}/folder-1/sub-folder-1",
                            Children =
                            [
                                new WebProjectStructureNode
                                {
                                    Id = "file-1",
                                    Name = "file",
                                    Path = "folder-1/sub-folder-1/file",
                                    AbsolutePath = $"{projectId}/folder-1/sub-folder-1/file",
                                },
                            ],
                        },
                    ],
                },
                new WebProjectStructureNode
                {
                    Id = "folder-2",
                    Name = "folder-2",
                    Path = "folder-2",
                    AbsolutePath = $"{projectId}/folder-2",
                    Children =
                    [
                        new WebProjectStructureNode
                        {
                            Id = "sub-folder-1",
                            Name = "sub-folder-1",
                            Path = "folder-2/sub-folder-1",
                            AbsolutePath = $"{projectId}/folder-2/sub-folder-1",
                            Children =
                            [
                                new WebProjectStructureNode
                                {
                                    Id = "file-2",
                                    Name = "file",
                                    Path = "folder-2/sub-folder-1/file",
                                    AbsolutePath = $"{projectId}/folder-2/sub-folder-1/file",
                                },
                            ],
                        },
                        new WebProjectStructureNode
                        {
                            Id = "file-2-1",
                            Name = "file-2",
                            Path = "folder-2/file-2",
                            AbsolutePath = $"{projectId}/folder-2/file-2",
                        },
                    ],
                },
            ])
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
