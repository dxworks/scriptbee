using System.Net.Sockets;

namespace ScriptBee.Rest.Tests;

public class ExceptionUtilsTest
{
    [Theory]
    [InlineData(SocketError.ConnectionRefused, true)]
    [InlineData(SocketError.HostNotFound, false)]
    public void IsConnectionRefused_ForSocketException_ReturnsExpectedResult(
        SocketError socketError,
        bool expected
    )
    {
        var socketException = new SocketException((int)socketError);

        var result = ExceptionUtils.IsConnectionRefused(socketException);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(SocketError.ConnectionRefused, true)]
    [InlineData(SocketError.HostNotFound, false)]
    public void IsConnectionRefused_ForHttpRequestExceptionWithInnerSocketException_ReturnsExpectedResult(
        SocketError innerSocketError,
        bool expected
    )
    {
        var innerSocketException = new SocketException((int)innerSocketError);
        var httpRequestException = new HttpRequestException("Error", innerSocketException);

        var result = ExceptionUtils.IsConnectionRefused(httpRequestException);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsConnectionRefused_ForHttpRequestExceptionWithoutInnerSocketException_ReturnsFalse()
    {
        var httpRequestException = new HttpRequestException("Error");

        var result = ExceptionUtils.IsConnectionRefused(httpRequestException);

        Assert.False(result);
    }

    [Fact]
    public void IsConnectionRefused_ForOtherExceptionType_ReturnsFalse()
    {
        var generalException = new Exception("General exception");

        var result = ExceptionUtils.IsConnectionRefused(generalException);

        Assert.False(result);
    }
}
