using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

public class UpdateProjectScriptsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IUpdateScriptUseCase, UpdateScriptService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/projects/{projectId}/scripts/{scriptId}", UpdateProjectScript)
            .WithTags("Scripts")
            .WithRequestValidation<WebUpdateScriptCommand>();

        app.MapPut(
                "/api/projects/{projectId}/scripts/{scriptId}/content",
                UpdateProjectScriptContent
            )
            .WithTags("Scripts");
    }

    private static async Task<
        Results<Ok<WebScriptData>, NotFound<ProblemDetails>>
    > UpdateProjectScript(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string scriptId,
        [FromBody] WebUpdateScriptCommand command,
        IUpdateScriptUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.Update(
            new UpdateScriptCommand(
                ProjectId.FromValue(projectId),
                new ScriptId(scriptId),
                command.Parameters?.Select(p => p.Map())
            ),
            cancellationToken
        );

        return result.Match<Results<Ok<WebScriptData>, NotFound<ProblemDetails>>>(
            script => TypedResults.Ok(WebScriptData.Map(script)),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }

    private static async Task<
        Results<NoContent, NotFound<ProblemDetails>>
    > UpdateProjectScriptContent(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string scriptId,
        [FromBody] string content,
        IUpdateScriptUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.UpdateContent(
            new UpdateScriptContentCommand(
                ProjectId.FromValue(projectId),
                new ScriptId(scriptId),
                content
            ),
            cancellationToken
        );

        return result.Match<Results<NoContent, NotFound<ProblemDetails>>>(
            _ => TypedResults.NoContent(),
            error => error.ToProblem(context),
            error => error.ToProblem(context)
        );
    }
}
