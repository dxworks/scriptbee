namespace ScriptBee.Common.Tests;

public class SecureRandomProviderTest
{
    private readonly SecureRandomProvider _provider = new();

    [Fact]
    public void GetBytes()
    {
        var guid = _provider.GetBytes(32);

        guid.Length.ShouldBe(32);
    }
}
