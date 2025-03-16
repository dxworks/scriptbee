using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using LinkResult = OneOf<Unit, InstanceDoesNotExistsError>;

public class LinkInstanceContextService(
    IGetProjectInstance getProjectInstance,
    IClearInstanceContext clearInstanceContext
) : ILinkInstanceContextUseCase
{
    public Task<LinkResult> Link(
        LinkContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
