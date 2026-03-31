using ScriptBee.Application.Model.Sorting;

namespace ScriptBee.Application.Model;

public record AnalysisSort(AnalysisSortField Field, SortOrder Order);

public enum AnalysisSortField
{
    CreationDate,
    FinishedDate,
    Status,
}
