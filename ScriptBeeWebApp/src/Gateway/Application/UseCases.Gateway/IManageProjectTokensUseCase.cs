using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway;

public interface IManageProjectTokensUseCase
{
    Task<List<ProjectToken>> GetProjectTokens(
        ProjectId projectId,
        CancellationToken cancellationToken
    );

    Task<NewProjectTokenResult> CreateProjectToken(
        CreateProjectTokenCommand command,
        CancellationToken cancellationToken
    );

    Task DeleteProjectToken(
        ProjectId projectId,
        ProjectTokenId id,
        CancellationToken cancellationToken
    );
}
