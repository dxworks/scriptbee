using System.Collections.Generic;

namespace ScriptBee.Services;

// todo add tests
public class ResultCollector : IResultCollector
{
    private readonly List<RunResult> _results = new();

    public void Add(string outputFileName, string type)
    {
        _results.Add(new RunResult(outputFileName, type));
    }

    public List<RunResult> GetResults()
    {
        return _results;
    }
}
