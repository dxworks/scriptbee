namespace ScriptBee.UseCases.Gateway;

public interface IRemoveProjectMemberUseCase
{
    Task RemoveProjectMember(
        RemoveProjectMemberCommand command,
        CancellationToken cancellationToken
    );
}
