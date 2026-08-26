using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class UpdateProjectMemberService(ISetResourceRole setResourceRole)
    : IUpdateProjectMemberUseCase
{
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
}
