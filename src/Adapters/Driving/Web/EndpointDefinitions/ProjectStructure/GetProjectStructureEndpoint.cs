using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class GetProjectStructureEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/structure", GetProjectStructure);
    }

    private static async Task<Ok<IEnumerable<WebProjectStructureNode>>> GetProjectStructure(
        [FromRoute] string projectId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok<IEnumerable<WebProjectStructureNode>>(
            [
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
            ]
        );
    }
}
