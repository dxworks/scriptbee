using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class ManageProjectTokensService() : IManageProjectTokensUseCase
{
    public Task<List<ProjectToken>> GetProjectTokens(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task<NewProjectTokenResult> CreateProjectToken(
        CreateProjectTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task DeleteProjectToken(
        ProjectId projectId,
        ProjectTokenId id,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
