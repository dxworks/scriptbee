namespace ScriptBee.Service.Gateway.Config;

public class ScriptBeeInstanceConfig
{
    public long PollingDelayMilliseconds { get; init; } = 1000;
    public int AnalysisStatusMonitorIntervalMilliseconds { get; init; } = 2000;
}
