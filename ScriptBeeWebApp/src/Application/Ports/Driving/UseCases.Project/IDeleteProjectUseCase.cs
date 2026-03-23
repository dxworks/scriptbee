namespace ScriptBee.UseCases.Project;

public interface IDeleteProjectUseCase
{
    Task DeleteProject(DeleteProjectCommand command, CancellationToken cancellationToken = default);
}
