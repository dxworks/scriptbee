namespace ScriptBee.Common;

public interface ISecureRandomProvider
{
    byte[] GetBytes(int count);
}
