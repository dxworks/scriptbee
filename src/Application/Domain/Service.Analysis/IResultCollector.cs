using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Service.Analysis;

public interface IResultCollector
{
    void Add(ResultId id, HelperFunctionsSettings settings, string outputFileName, string type);
}
