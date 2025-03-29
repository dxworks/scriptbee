using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using LinkContextResult = OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>;

public class LinkInstanceContextService(
    IGetProject getProject,
    IGetProjectInstance getProjectInstance,
    ILinkInstanceContext linkInstanceContext,
    IUpdateProject updateProject
) : ILinkInstanceContextUseCase
{
    public async Task<LinkContextResult> Link(
        LinkContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<LinkContextResult>>(
            async projectDetails =>
                await Link(
                    projectDetails,
                    command.InstanceId,
                    command.LinkerIds,
                    cancellationToken
                ),
            error => Task.FromResult<LinkContextResult>(error)
        );
    }

    private async Task<LinkContextResult> Link(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        IEnumerable<string> linkerIds,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProjectInstance.Get(instanceId, cancellationToken);

        return await result.Match<Task<LinkContextResult>>(
            async instanceInfo =>
            {
                await Link(projectDetails, instanceInfo, linkerIds.ToList(), cancellationToken);
                return new Unit();
            },
            error => Task.FromResult<LinkContextResult>(error)
        );
    }

    private async Task Link(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        List<string> linkerIds,
        CancellationToken cancellationToken
    )
    {
        await linkInstanceContext.Link(instanceInfo, linkerIds, cancellationToken);

        await updateProject.Update(projectDetails with { Linkers = linkerIds }, cancellationToken);
    }
}
