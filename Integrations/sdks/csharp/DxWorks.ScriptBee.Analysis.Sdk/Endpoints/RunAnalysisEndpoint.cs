using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Validation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class RunAnalysisEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/analyses", TriggerAnalysis)
            .WithTags("Analysis")
            .WithSummary("Run analysis")
            .WithDescription("Starts the execution of an analysis script on the analysis service.")
            .WithRequestValidation<WebRunAnalysisCommand>()
            .WithName("Analyses");
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
