using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class RemoveProjectMemberService(IRemoveProjectMember removeProjectMember)
    : IRemoveProjectMemberUseCase
{
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
}
