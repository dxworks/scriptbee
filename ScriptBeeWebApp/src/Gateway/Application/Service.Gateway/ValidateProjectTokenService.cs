using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class ValidateProjectTokenService(IGetProjectTokenByHash getProjectTokenByHash)
    : IValidateProjectTokenUseCase
{
    public async Task<ProjectToken?> ValidateToken(
        string rawToken,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(rawToken)
            || !rawToken.StartsWith(ProjectToken.Prefix, StringComparison.Ordinal)
        )
        {
            return null;
        }

        var tokenHash = ProjectToken.ComputeHash(rawToken);
        var token = await getProjectTokenByHash.GetTokenByHash(tokenHash, cancellationToken);
        if (token is null)
        {
            return null;
        }

        return token.ExpiresAt <= DateTimeOffset.UtcNow ? null : token;
    }
}
