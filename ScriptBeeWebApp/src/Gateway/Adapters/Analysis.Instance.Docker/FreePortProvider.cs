using System.Net;
using System.Net.Sockets;

namespace ScriptBee.Analysis.Instance.Docker;

public class FreePortProvider : IFreePortProvider
{
    public int GetFreeTcpPort()
    {
        var l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        var port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }
}
