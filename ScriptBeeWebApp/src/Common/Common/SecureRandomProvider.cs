using System.Security.Cryptography;

namespace ScriptBee.Common;

public sealed class SecureRandomProvider : ISecureRandomProvider
{
    public byte[] GetBytes(int count) => RandomNumberGenerator.GetBytes(32);
}
