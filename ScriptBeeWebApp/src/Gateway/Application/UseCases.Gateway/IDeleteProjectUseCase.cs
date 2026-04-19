namespace ScriptBee.UseCases.Gateway;

public interface IDeleteProjectUseCase
{
    Task DeleteProject(DeleteProjectCommand command, CancellationToken cancellationToken = default);
}
