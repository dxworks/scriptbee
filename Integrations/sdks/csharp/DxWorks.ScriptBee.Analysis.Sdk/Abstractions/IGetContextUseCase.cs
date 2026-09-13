using ScriptBee.Domain.Model.Context;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface IGetContextUseCase
{
    IEnumerable<ContextSlice> Get();
}
