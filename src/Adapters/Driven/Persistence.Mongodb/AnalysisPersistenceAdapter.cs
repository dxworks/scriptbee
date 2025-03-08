using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Entity.Analysis;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Analysis;

namespace ScriptBee.Persistence.Mongodb;

public class AnalysisPersistenceAdapter(IMongoRepository<MongodbAnalysisInfo> mongoRepository)
    : IGetAnalysis,
        IGetAllAnalyses,
        ICreateAnalysis,
        IUpdateAnalysis,
        IDeleteAnalysis
{
    public async Task<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        var analysisInfo = await mongoRepository.GetDocument(
            analysisId.ToString(),
            cancellationToken
        );

        if (analysisInfo == null)
        {
            return new AnalysisDoesNotExistsError(analysisId);
        }

        return analysisInfo.ToAnalysisInfo();
    }

    public async Task<IEnumerable<AnalysisInfo>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var analysisInfos = await mongoRepository.GetAllDocuments(
            instance => instance.ProjectId == projectId.Value,
            cancellationToken
        );

        return analysisInfos.Select(analysisInfo => analysisInfo.ToAnalysisInfo());
    }

    public async Task<AnalysisInfo> Create(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken = default
    )
    {
        await mongoRepository.CreateDocument(
            MongodbAnalysisInfo.From(analysisInfo),
            cancellationToken
        );
        return analysisInfo;
    }

    public async Task<AnalysisInfo> Update(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken = default
    )
    {
        await mongoRepository.UpdateDocument(
            MongodbAnalysisInfo.From(analysisInfo),
            cancellationToken
        );
        return analysisInfo;
    }

    public async Task DeleteById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        await mongoRepository.DeleteDocument(analysisId.ToString(), cancellationToken);
    }

    public async Task DeleteAllByProjectId(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        await mongoRepository.DeleteDocument(
            d => d.ProjectId == projectId.ToString(),
            cancellationToken
        );
    }
}
