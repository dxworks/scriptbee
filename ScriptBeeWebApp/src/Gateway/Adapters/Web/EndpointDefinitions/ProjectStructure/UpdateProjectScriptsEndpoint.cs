using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Gateway.ProjectStructure;
using ScriptBee.UseCases.Gateway.ProjectStructure;
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
            .WithSummary("Update project script metadata")
            .WithDescription(
                "Updates the metadata (name, parameters) of a specific project script."
            )
            .WithRequestValidation<WebUpdateScriptCommand>();

        app.MapPut(
                "/api/projects/{projectId}/scripts/{scriptId}/content",
                UpdateProjectScriptContent
            )
            .Accepts<string>(MediaTypeNames.Text.Plain)
            .WithTags("Scripts")
            .WithSummary("Update project script content")
            .WithDescription("Updates the actual code content of a specific project script.");
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
                command.Name,
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
        Stream bodyStream,
        IUpdateScriptUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        using var reader = new StreamReader(bodyStream);
        var content = await reader.ReadToEndAsync(cancellationToken);

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
