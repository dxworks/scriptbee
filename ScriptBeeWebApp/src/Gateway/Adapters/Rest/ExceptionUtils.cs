using System.Net.Sockets;

namespace ScriptBee.Rest;

public static class ExceptionUtils
{
    public static bool IsConnectionRefused(Exception ex)
    {
        return ex switch
        {
            SocketException socketEx => socketEx.SocketErrorCode == SocketError.ConnectionRefused,

            HttpRequestException { InnerException: SocketException innerSocketEx } =>
                innerSocketEx.SocketErrorCode == SocketError.ConnectionRefused,
            _ => false,
        };
    }
}
