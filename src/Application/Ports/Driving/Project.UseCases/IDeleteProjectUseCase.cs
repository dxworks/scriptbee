namespace ScriptBee.Project.UseCases;

public interface IDeleteProjectUseCase
{
    Task DeleteProject(DeleteProjectCommand command, CancellationToken cancellationToken = default);
}
