using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Service.Analysis;

public class ResultCollector(IDateTimeProvider dateTimeProvider) : IResultCollector
{
    private readonly List<ResultSummary> _results = [];

    public void Add(
        ResultId id,
        // TODO FIXIT(#20): remove settings if not needed
        HelperFunctionsSettings settings,
        string outputFileName,
        string type
    )
    {
        _results.Add(new ResultSummary(id, outputFileName, type, dateTimeProvider.UtcNow()));
    }

    public List<ResultSummary> GetResults()
    {
        return _results;
    }
}
