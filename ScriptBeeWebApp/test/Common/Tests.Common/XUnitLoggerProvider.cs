using Microsoft.Extensions.Logging;

namespace ScriptBee.Tests.Common;

public class XUnitLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger<ILoggerProvider>(output, categoryName);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
