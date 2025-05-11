using Microsoft.Extensions.Logging;

namespace ScriptBee.Tests.Common;

public class XUnitLogger<T>(ITestOutputHelper output, string categoryName) : ILogger<T>
{
    public XUnitLogger(ITestOutputHelper output)
        : this(output, typeof(T).FullName ?? string.Empty) { }

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
