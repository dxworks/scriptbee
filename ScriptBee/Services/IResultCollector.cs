using System;

namespace ScriptBee.Services;

public interface IResultCollector
{
    void Add(Guid id, int runIndex, string outputFileName, string type);
}
