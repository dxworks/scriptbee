using System.Security.Cryptography;
using System.Text;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway;

public sealed class ManageProjectTokensService(
    ISecureRandomProvider secureRandomProvider,
    IGetAllProjectTokens getAllProjectTokens,
    ICreateProjectToken createProjectToken,
    IDeleteProjectToken deleteProjectToken
) : IManageProjectTokensUseCase
{
    private const string Prefix = "sb_at_";
    private const int TokenRandomBytesSize = 32;

    public async Task<List<ProjectToken>> GetProjectTokens(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        return await getAllProjectTokens.GetAllForProjectId(projectId, cancellationToken);
    }

    public async Task<NewProjectTokenResult> CreateProjectToken(
        CreateProjectTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var (rawToken, tokenHash) = GenerateToken();

        var token = await createProjectToken.CreateToken(
            command.ProjectId,
            tokenHash,
            command.Description,
            command.Role,
            command.ExpiresAt,
            cancellationToken
        );

        return new NewProjectTokenResult(token, rawToken);
    }

    public async Task DeleteProjectToken(
        ProjectId projectId,
        ProjectTokenId id,
        CancellationToken cancellationToken
    )
    {
        await deleteProjectToken.DeleteToken(projectId, id, cancellationToken);
    }

    private (string rawToken, string tokenHash) GenerateToken()
    {
        var randomBytes = secureRandomProvider.GetBytes(TokenRandomBytesSize);

        var secretPayload = Convert
            .ToBase64String(randomBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        var rawToken = $"{Prefix}{secretPayload}";

        var tokenHash = ComputeHash(rawToken);

        return (rawToken, tokenHash);
    }

    private static string ComputeHash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}
