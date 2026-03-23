using ScriptBee.Domain.Model.Context;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class GetContextService(IProjectManager projectManager) : IGetContextUseCase
{
    public IEnumerable<ContextSlice> Get()
    {
        var project = projectManager.GetProject();

        return project
            .Context.Models.Keys.GroupBy(tuple => tuple.Item1)
            .Select(grouping => new ContextSlice(
                grouping.Key,
                grouping.Select(tuple => tuple.Item2).ToList()
            ));
    }
}
