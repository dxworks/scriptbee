using ScriptBee.Domain.Model.Project;

namespace ScriptBee.UseCases.Gateway;

public interface IValidateProjectTokenUseCase
{
    Task<ProjectToken?> ValidateToken(string rawToken, CancellationToken cancellationToken);
}
