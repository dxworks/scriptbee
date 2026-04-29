using System.Linq.Expressions;
using MongoDB.Driver;
using OneOf;
using ScriptBee.Analysis.Mongodb.Entity;
using ScriptBee.Application.Model;
using ScriptBee.Application.Model.Sorting;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Analysis.Mongodb;

public class AnalysisPersistenceAdapter(IMongoRepository<MongodbAnalysisInfo> mongoRepository)
    : IGetAnalysis,
        IGetAllAnalyses,
        IGetRunningAnalyses,
        ICreateAnalysis,
        IUpdateAnalysis,
        IDeleteAnalysis
{
    public async Task<IEnumerable<AnalysisInfo>> GetRunning(CancellationToken cancellationToken)
    {
        var runningStatuses = new[] { AnalysisStatus.Started.Value, AnalysisStatus.Running.Value };

        var analysisInfos = await mongoRepository.GetAllDocuments(
            instance => runningStatuses.AsEnumerable().Contains(instance.Status),
            null,
            cancellationToken
        );

        return analysisInfos.Select(analysisInfo => analysisInfo.ToAnalysisInfo());
    }

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
        IReadOnlyList<AnalysisSort> sorts,
        CancellationToken cancellationToken
    )
    {
        var sortDefinition = BuildSortDefinition(sorts);

        var analysisInfos = await mongoRepository.GetAllDocuments(
            instance => instance.ProjectId == projectId.Value,
            sortDefinition,
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

    private static SortDefinition<MongodbAnalysisInfo>? BuildSortDefinition(
        IReadOnlyList<AnalysisSort> sorts
    )
    {
        if (sorts.Count == 0)
        {
            return null;
        }

        var builder = Builders<MongodbAnalysisInfo>.Sort;

        var map = new Dictionary<AnalysisSortField, Expression<Func<MongodbAnalysisInfo, object>>>
        {
            { AnalysisSortField.CreationDate, x => x.CreationDate },
            { AnalysisSortField.FinishedDate, x => x.FinishedDate! },
            { AnalysisSortField.Status, x => x.Status },
        };

        var definitions = sorts
            .Where(s => map.ContainsKey(s.Field))
            .Select(s =>
                s.Order == SortOrder.Ascending
                    ? builder.Ascending(map[s.Field])
                    : builder.Descending(map[s.Field])
            )
            .ToList();

        return definitions.Count > 0 ? builder.Combine(definitions) : null;
    }
}
