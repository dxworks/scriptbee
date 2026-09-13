using ScriptBee.Domain.Model.Context;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface IGetContextGraphUseCase
{
    ContextGraphResult SearchNodes(string query, int offset, int limit);

    ContextGraphResult GetNeighbors(string nodeId);
}
