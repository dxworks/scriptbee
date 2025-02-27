using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class GetAnalysisService(
    IGetAllAnalysisResults getAllAnalysisResults,
    IGetAnalysisResult getAnalysisResult
) : IGetAnalysisUseCase
{
    public async Task<IEnumerable<AnalysisResult>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        return await getAllAnalysisResults.GetAll(projectId, cancellationToken);
    }

    public async Task<OneOf<AnalysisResult, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        return await getAnalysisResult.GetById(analysisId, cancellationToken);
    }
}
