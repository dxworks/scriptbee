using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class GetProjectScriptsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        // TODO FIXIT: update dependencies
        services.AddSingleton<IGetScriptsUseCase, GetScriptsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/scripts", GetProjectScripts).WithTags("Scripts");
        app.MapGet("/api/projects/{projectId}/scripts/{scriptId}", GetProjectScriptById)
            .WithTags("Scripts");
        app.MapGet("/api/projects/{projectId}/scripts/{scriptId}/content", GetProjectScriptsContent)
            .WithTags("Scripts");
    }

    private static async Task<Ok<WebGetScriptDataResponse>> GetProjectScripts(
        [FromRoute] string projectId,
        IGetScriptsUseCase useCase,
        CancellationToken cancellation
    )
    {
        var scripts = await useCase.GetAll(ProjectId.FromValue(projectId), cancellation);

        return TypedResults.Ok(
            new WebGetScriptDataResponse(scripts.Select(WebScriptData.Map).ToList())
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
