namespace ScriptBee.Calculation.Instance.Docker;

public class CalculationDockerConfig
{
    public required string DockerSocket { get; init; }

    public required int Port { get; init; }

    public required string Network { get; init; }
}
