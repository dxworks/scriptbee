using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class UpdateProjectScriptsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/projects/{projectId}/scripts", UpdateProjectScript);
    }

    private static async Task<Ok<WebScriptData>> UpdateProjectScript(
        [FromRoute] string projectId,
        // TODO FIXIT: add validation
        [FromBody]
            WebUpdateScriptCommand command
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
}
