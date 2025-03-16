using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using LinkContextResult = OneOf<Unit, InstanceDoesNotExistsError>;

public class LinkInstanceContextService(
    IGetProjectInstance getProjectInstance,
    ILinkInstanceContext linkInstanceContext
) : ILinkInstanceContextUseCase
{
    public async Task<LinkContextResult> Link(
        LinkContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(command.InstanceId, cancellationToken);

        return await result.Match<Task<LinkContextResult>>(
            async instanceInfo =>
            {
                await linkInstanceContext.Link(instanceInfo, command.LinkerIds, cancellationToken);
                return new Unit();
            },
            error => Task.FromResult<LinkContextResult>(error)
        );
    }
}
