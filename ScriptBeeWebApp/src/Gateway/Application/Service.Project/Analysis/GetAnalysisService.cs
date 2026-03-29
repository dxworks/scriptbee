using OneOf;
using ScriptBee.Analysis;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

public class GetAnalysisService(IGetAllAnalyses getAllAnalyses, IGetAnalysis getAnalysis)
    : IGetAnalysisUseCase
{
    public async Task<IEnumerable<AnalysisInfo>> GetAll(
        ProjectId projectId,
        SortOrder sortOrder,
        CancellationToken cancellationToken = default
    )
    {
        return await getAllAnalyses.GetAll(projectId, sortOrder, cancellationToken);
    }

    public async Task<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        return await getAnalysis.GetById(analysisId, cancellationToken);
    }
}
