using ScriptBee.Domain.Model.Context;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class GetContextService() : IGetContextUseCase
{
    public IEnumerable<ContextSlice> Get()
    {
        throw new NotImplementedException();
    }
}
