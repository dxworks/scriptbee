﻿using MongoDB.Bson.Serialization.Attributes;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Entity.Analysis;

public class MongodbAnalysisInfo : IDocument
{
    [BsonId]
    public required string Id { get; set; }
    public required string ProjectId { get; set; }
    public required string ScriptId { get; set; }
    public required int Status { get; set; }
    public required IEnumerable<MongodbResultSummary> Results { get; set; }
    public required IEnumerable<MongodbAnalysisError> Errors { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    public required DateTimeOffset? FinishedDate { get; set; }

    public AnalysisInfo ToAnalysisInfo()
    {
        return new AnalysisInfo(
            new AnalysisId(Id),
            Domain.Model.Project.ProjectId.FromValue(ProjectId),
            new ScriptId(ScriptId),
            (AnalysisStatus)Status,
            Results.Select(r => r.ToResultSummary()),
            Errors.Select(e => e.ToAnalysisError()),
            CreationDate,
            FinishedDate
        );
    }

    public static MongodbAnalysisInfo From(AnalysisInfo analysisInfo)
    {
        return new MongodbAnalysisInfo
        {
            Id = analysisInfo.Id.ToString(),
            ProjectId = analysisInfo.ProjectId.ToString(),
            ScriptId = analysisInfo.ScriptId.ToString(),
            Status = (int)analysisInfo.Status,
            Results = analysisInfo.Results.Select(MongodbResultSummary.From),
            Errors = analysisInfo.Errors.Select(MongodbAnalysisError.From),
            CreationDate = analysisInfo.CreationDate,
            FinishedDate = analysisInfo.FinishedDate,
        };
    }
}
