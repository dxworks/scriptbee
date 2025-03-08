using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class GetProjectScriptsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/scripts", GetProjectScripts);
        app.MapGet("/api/projects/{projectId}/scripts/{scriptId}", GetProjectScriptById);
        app.MapGet(
            "/api/projects/{projectId}/scripts/{scriptId}/content",
            GetProjectScriptsContent
        );
    }

    private static async Task<Ok<WebGetScriptDataResponse>> GetProjectScripts(
        [FromRoute] string projectId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok(
            new WebGetScriptDataResponse(
                [
                    new WebScriptData
                    {
                        Id = "file-1",
                        Name = "file",
                        Path = "folder-1/sub-folder-1/file",
                        AbsolutePath = $"{projectId}/folder-1/sub-folder-1/file",
                        ScriptLanguage = new WebScriptLanguage("csharp", ".cs"),
                        Parameters = [new WebScriptParameter("param-1", "string", "hello")],
                    },
                    new WebScriptData
                    {
                        Id = "file-2",
                        Name = "file",
                        Path = "folder-2/sub-folder-1/file",
                        AbsolutePath = $"{projectId}/folder-2/sub-folder-1/file",
                        ScriptLanguage = new WebScriptLanguage("python", ".py"),
                        Parameters =
                        [
                            new WebScriptParameter("param", "boolean", true),
                            new WebScriptParameter("param2", "integer", 124),
                        ],
                    },
                    new WebScriptData
                    {
                        Id = "file-2-1",
                        Name = "file-2",
                        Path = "folder-2/file-2",
                        AbsolutePath = $"{projectId}/folder-2/file-2",
                        ScriptLanguage = new WebScriptLanguage("javascript", ".js"),
                        Parameters = [],
                    },
                ]
            )
        );
    }

    private static async Task<Ok<WebScriptData>> GetProjectScriptById(
        [FromRoute] string projectId,
        [FromRoute] string scriptId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok(
            new WebScriptData
            {
                Id = "file-1",
                Name = "file",
                Path = "folder-1/sub-folder-1/file",
                AbsolutePath = $"{projectId}/folder-1/sub-folder-1/file",
                ScriptLanguage = new WebScriptLanguage("csharp", ".cs"),
                Parameters = [new WebScriptParameter("param-1", "string", "hello")],
            }
        );
    }

    private static async Task<Ok<string>> GetProjectScriptsContent(
        [FromRoute] string projectId,
        [FromRoute] string scriptId
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value
        return TypedResults.Ok(scriptId);
    }
}
