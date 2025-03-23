using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Domain.Model.Analysis;

public record ResultSummary(
    ResultId Id,
    ProjectId ProjectId,
    AnalysisId AnalysisId,
    string Type,
    DateTimeOffset CreationDate
);
