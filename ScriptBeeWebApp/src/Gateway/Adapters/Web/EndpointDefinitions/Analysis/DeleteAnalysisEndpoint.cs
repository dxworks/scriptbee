using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis;

public class DeleteAnalysisEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IDeleteAnalysisUseCase, DeleteAnalysisService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/projects/{projectId}/analyses/{analysisId}", DeleteAnalysis)
            .WithTags("Analysis")
            .WithSummary("Delete analysis")
            .WithDescription("Deletes a specific analysis and all its associated artifacts.");
    }

    private static async Task<NoContent> DeleteAnalysis(
        [FromRoute] string projectId,
        [FromRoute] string analysisId,
        IDeleteAnalysisUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        await useCase.Delete(
            new DeleteAnalysisCommand(ProjectId.FromValue(projectId), new AnalysisId(analysisId)),
            cancellationToken
        );
        return TypedResults.NoContent();
    }
}
