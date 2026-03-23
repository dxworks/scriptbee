namespace ScriptBee.Analysis.Instance.Docker.Tests;

public class FreePortProviderTest
{
    private readonly FreePortProvider _freePortProvider = new();

    [Fact]
    public void PortIsNotZero()
    {
        var port = _freePortProvider.GetFreeTcpPort();

        port.ShouldBeGreaterThan(0);
    }
}
