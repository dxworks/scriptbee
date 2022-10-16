using System;
using System.Collections.Generic;
using ScriptBee.Models;
using ScriptBee.Services;

namespace ScriptBeeWebApp.Services;

// todo add tests
public class ResultCollector : IResultCollector
{
    private readonly List<RunResult> _results = new();

    public void Add(Guid id, int runIndex, string outputFileName, string type)
    {
        _results.Add(new RunResult
        {
            Id = id,
            Name = outputFileName,
            RunIndex = runIndex,
            Type = type
        });
    }

    public List<RunResult> GetResults()
    {
        return _results;
    }
}
