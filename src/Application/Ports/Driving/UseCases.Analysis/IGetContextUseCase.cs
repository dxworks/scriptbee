using ScriptBee.Domain.Model.Context;

namespace ScriptBee.UseCases.Analysis;

public interface IGetContextUseCase
{
    IEnumerable<ContextSlice> Get();
}
