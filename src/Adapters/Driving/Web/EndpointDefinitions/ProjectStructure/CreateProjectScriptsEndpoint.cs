using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class CreateProjectScriptsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/scripts", CreateProjectScript);
    }

    // TODO FIXIT: return 409 for conflict if needed
    private static async Task<Ok<WebScriptData>> CreateProjectScript(
        [FromRoute] string projectId,
        // TODO FIXIT: add validation
        [FromBody]
            WebCreateScriptCommand command
    )
    {
        await Task.CompletedTask;

        // TODO FIXIT: remove hardcoded value

        return TypedResults.Ok(
            new WebScriptData
            {
                Id = command.Path,
                Name = command.Path,
                Path = command.Path,
                AbsolutePath = $"{projectId}/${command.Path}",
                ScriptLanguage = new WebScriptLanguage(command.Language, ".cs"),
                Parameters = command.Parameters ?? [],
            }
        );
    }
}
