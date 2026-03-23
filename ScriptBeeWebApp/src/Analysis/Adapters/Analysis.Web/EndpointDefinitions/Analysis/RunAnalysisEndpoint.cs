using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Analysis.Web.EndpointDefinitions.Analysis.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Analysis;

public class RunAnalysisEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IRunAnalysisUseCase, RunAnalysisService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/analyses", TriggerAnalysis)
            .WithRequestValidation<WebRunAnalysisCommand>();
    }

    private static async Task<Accepted<WebRunAnalysisResponse>> TriggerAnalysis(
        [FromBody] WebRunAnalysisCommand command,
        IRunAnalysisUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var runAnalysis = new RunAnalysisCommand(
            ProjectId.FromValue(command.ProjectId),
            new ScriptId(command.ScriptId)
        );
        var analysisInfo = await useCase.Run(runAnalysis, cancellationToken);

        return TypedResults.Accepted(
            $"/api/analyses/{analysisInfo.Id}",
            WebRunAnalysisResponse.FromAnalysisInfo(analysisInfo)
        );
    }
}
