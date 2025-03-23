using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using LoadContextResult = OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>;

public class LoadInstanceContextService(
    IGetProjectInstance getProjectInstance,
    ILinkInstanceContext linkInstanceContext
) : ILoadInstanceContextUseCase
{
    public async Task<LoadContextResult> Load(
        LoadContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(command.InstanceId, cancellationToken);

        return await result.Match<Task<LoadContextResult>>(
            async instanceInfo =>
            {
                await linkInstanceContext.Link(instanceInfo, command.LoaderIds, cancellationToken);
                return new Unit();
            },
            error => Task.FromResult<LoadContextResult>(error)
        );
    }
}
