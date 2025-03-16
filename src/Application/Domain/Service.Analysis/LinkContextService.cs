using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class LinkContextService(IProjectManager projectManager) : ILinkContextUseCase
{
    public Task Link(IEnumerable<string> linkerIds, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
