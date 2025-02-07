using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ScriptBee.Gateway.Persistence.Mongodb.Tests;

public class TestLogger : ILogger
{
    private readonly Logger _logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

    public void Write(LogEvent logEvent)
    {
        _logger.Write(logEvent);
    }
}
