using System.Security.Cryptography;
using System.Text;
using ScriptBee.Domain.Model.User;

namespace ScriptBee.Domain.Model.Project;

public record ProjectToken(
    ProjectTokenId Id,
    ProjectId ProjectId,
    string TokenHash,
    string? Description,
    UserRole Role,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt
)
{
    public const string Prefix = "sb_at_";

    public static string ComputeHash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}
