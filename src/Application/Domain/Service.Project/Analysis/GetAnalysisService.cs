using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Analysis;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class GetAnalysisService(IGetAllAnalyses getAllAnalyses, IGetAnalysis getAnalysis)
    : IGetAnalysisUseCase
{
    public async Task<IEnumerable<AnalysisInfo>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        return await getAllAnalyses.GetAll(projectId, cancellationToken);
    }

    public async Task<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        return await getAnalysis.GetById(analysisId, cancellationToken);
    }
}
