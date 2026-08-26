namespace ScriptBee.UseCases.Gateway;

public interface IUpdateProjectMemberUseCase
{
    Task UpdateProjectMember(
        UpdateProjectMemberCommand command,
        CancellationToken cancellationToken
    );
}
