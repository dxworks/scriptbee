namespace ScriptBee.Common.Tests;

public class GuidProviderTest
{
    private readonly GuidProvider _guidProvider = new();

    [Fact]
    public void NewGuid()
    {
        var guid = _guidProvider.NewGuid();

        guid.ShouldNotBe(Guid.Empty);
    }
}
