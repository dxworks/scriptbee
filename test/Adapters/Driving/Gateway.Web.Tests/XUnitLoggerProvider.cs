using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests;

public class XUnitLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(output, categoryName);
    }

    public void Dispose()
    {
        // No resources to dispose
    }

    private class XUnitLogger(ITestOutputHelper output, string categoryName) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            var message = formatter(state, exception);

            output.WriteLine($"[{logLevel}] {categoryName}: {message} {exception}");
        }

        public bool IsEnabled(LogLevel logLevel) => true;
    }
}
