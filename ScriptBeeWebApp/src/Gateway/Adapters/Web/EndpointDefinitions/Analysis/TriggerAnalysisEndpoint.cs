using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Analysis;

public class TriggerAnalysisEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ITriggerAnalysisUseCase, TriggerAnalysisService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/instances/{instanceId}/analyses", TriggerAnalysis)
            .WithTags("Instances", "Analysis")
            .WithRequestValidation<WebTriggerAnalysisCommand>();
    }

    private static async Task<Results<Accepted, NotFound<ProblemDetails>>> TriggerAnalysis(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string instanceId,
        [FromBody] WebTriggerAnalysisCommand command,
        ITriggerAnalysisUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.Trigger(
            new TriggerAnalysisCommand(
                ProjectId.FromValue(projectId),
                new InstanceId(instanceId),
                new ScriptId(command.ScriptId)
            ),
            cancellationToken
        );

        return result.Match<Results<Accepted, NotFound<ProblemDetails>>>(
            info => TypedResults.Accepted($"/api/projects/{info.ProjectId}/analyses/{info.Id}"),
            error => error.ToProblem(context)
        );
    }
}
