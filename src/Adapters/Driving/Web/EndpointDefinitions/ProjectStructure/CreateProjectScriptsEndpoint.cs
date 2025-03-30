using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
using ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure;

using CreateResponse = Results<
    Created<WebScriptData>,
    NotFound<ProblemDetails>,
    ValidationProblem,
    Conflict<ProblemDetails>
>;

public class CreateProjectScriptsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ICreateScriptUseCase, CreateScriptService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/scripts", CreateProjectScript)
            .WithRequestValidation<WebCreateScriptCommand>();
    }

    private static async Task<CreateResponse> CreateProjectScript(
        HttpContext context,
        [FromRoute] string projectId,
        [FromBody] WebCreateScriptCommand command,
        ICreateScriptUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.Create(
            command.Map(ProjectId.FromValue(projectId)),
            cancellationToken
        );

        return result.Match<CreateResponse>(
            script =>
                TypedResults.Created(
                    $"/api/projects/{projectId}/scripts/{script.Id}",
                    WebScriptData.Map(script)
                ),
            error => error.ToProblem(context),
            error =>
                TypedResults.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        {
                            nameof(WebCreateScriptCommand.Language),
                            [$"'{error.Language}' language does not exists."]
                        },
                    }
                ),
            _ =>
                TypedResults.Conflict(
                    context.ToProblemDetails(
                        "Script Path Already Exists",
                        "A script at that path already exists."
                    )
                )
        );
    }
}
