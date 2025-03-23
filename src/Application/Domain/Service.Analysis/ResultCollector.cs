using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Service.Analysis;

public class ResultCollector(IDateTimeProvider dateTimeProvider) : IResultCollector
{
    private readonly List<ResultSummary> _results = [];

    public void Add(
        ResultId id,
        HelperFunctionsSettings settings,
        string outputFileName,
        string type
    )
    {
        _results.Add(
            new ResultSummary(
                id,
                settings.ProjectId,
                settings.AnalysisId,
                type,
                dateTimeProvider.UtcNow()
            )
        );
    }

    public List<ResultSummary> GetResults()
    {
        return _results;
    }
}
