namespace ScriptBee.Common.Web.Extensions;

public sealed class TelemetryOptions
{
    public bool Enabled { get; init; } = true;
    public string? OtlpEndpoint { get; init; }
    public bool ConsoleExporterEnabled { get; init; } = false;
}
