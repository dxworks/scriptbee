using ScriptBee.Domain.Model.Context;

namespace ScriptBee.UseCases.Analysis;

public interface IGetContextGraphUseCase
{
    ContextGraphResult SearchNodes(string query, int offset, int limit);

    ContextGraphResult GetNeighbors(string nodeId);
}
