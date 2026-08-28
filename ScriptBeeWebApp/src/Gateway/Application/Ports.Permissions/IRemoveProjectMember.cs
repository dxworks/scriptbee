using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Ports.Permissions;

public interface IRemoveProjectMember
{
    Task RemoveProjectMember(
        ProjectId projectId,
        string memberId,
        string memberType,
        CancellationToken cancellationToken
    );

    Task RemoveAllProjectMembers(ProjectId projectId, CancellationToken cancellationToken);
}
