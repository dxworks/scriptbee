using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Service.Analysis;

public interface IResultCollector
{
    void Add(ResultId id, string outputFileName, string type);
}
