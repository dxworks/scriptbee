using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ScriptBee.Tests.Common;

public class XunitLogger<T>(ITestOutputHelper outputHelper) : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return new NoopDisposable();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        var logString = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}";

        if (exception != null)
        {
            logString += Environment.NewLine + exception;
        }

        outputHelper.WriteLine(logString);
    }

    private class NoopDisposable : IDisposable
    {
        public void Dispose() { }
    }
}
