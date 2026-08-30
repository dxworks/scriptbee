using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Service.Gateway.Config;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class ManageUsersService(
    IOptions<ScriptBeeUserManagementConfig> userManagementConfigOptions,
    IMemoryCache cache,
    IGetAvailableRoles getAvailableRoles,
    IGetOrAddUser getOrAddUser,
    IGetAllUsers getAllUsers,
    IGetProjectMembers getProjectMembers,
    ISetResourceRole setResourceRole,
    IRemoveProjectMember removeProjectMember
) : IManageUsersUseCase
{
    public Task<List<RoleInfo>> GetAvailableRoles(CancellationToken cancellationToken)
    {
        return getAvailableRoles.GetRoles(cancellationToken);
    }

    public async Task<UserId> GetUserId(
        string externalUserId,
        string externalUserName,
        CancellationToken cancellationToken
    )
    {
        var userId = await cache.GetOrCreateAsync(
            externalUserId,
            _ => getOrAddUser.GetOrAddUser(externalUserId, externalUserName, cancellationToken),
            GetCacheOptions()
        );

        return userId;
    }

    public Task<List<UserInfo>> GetAllUsers(CancellationToken cancellationToken)
    {
        return getAllUsers.GetAllUsers(cancellationToken);
    }

    public Task<List<ProjectMember>> GetProjectMembers(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        return getProjectMembers.GetProjectMembers(projectId, cancellationToken);
    }

    public Task RemoveProjectMember(
        RemoveProjectMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        return removeProjectMember.RemoveProjectMember(
            command.ProjectId,
            command.MemberId,
            command.MemberType,
            cancellationToken
        );
    }

    public Task UpdateProjectMember(
        UpdateProjectMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        return setResourceRole.SetRoleForMember(
            command.MemberId,
            command.MemberType,
            command.ProjectId,
            command.Role,
            cancellationToken
        );
    }

    private MemoryCacheEntryOptions GetCacheOptions()
    {
        var config = userManagementConfigOptions.Value;
        return new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(config.UserInfoCacheTimeSeconds),
        };
    }
}
