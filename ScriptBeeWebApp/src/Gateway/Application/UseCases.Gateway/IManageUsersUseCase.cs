using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.UseCases.Gateway;

public interface IManageUsersUseCase
{
    Task<List<RoleInfo>> GetAvailableRoles(CancellationToken cancellationToken);

    Task<UserId> GetUserId(
        string externalUserId,
        string externalUserName,
        CancellationToken cancellationToken
    );

    Task<List<UserInfo>> GetAllUsers(CancellationToken cancellationToken);

    Task<List<ProjectMember>> GetProjectMembers(
        ProjectId projectId,
        CancellationToken cancellationToken
    );

    Task RemoveProjectMember(
        RemoveProjectMemberCommand command,
        CancellationToken cancellationToken
    );

    Task UpdateProjectMember(
        UpdateProjectMemberCommand command,
        CancellationToken cancellationToken
    );
}
