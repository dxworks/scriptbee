using System.Collections.Generic;
using ScriptBee.Models;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

// todo add tests
public class ResultCollector : IResultCollector
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly List<RunResult> _results = new();

    public ResultCollector(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public void Add(string outputFileName, string type)
    {
        _results.Add(new RunResult
        {
            Id = _guidGenerator.GenerateGuid(),
            Path = outputFileName,
            Type = type
        });
    }

    public List<RunResult> GetResults()
    {
        return _results;
    }
}
