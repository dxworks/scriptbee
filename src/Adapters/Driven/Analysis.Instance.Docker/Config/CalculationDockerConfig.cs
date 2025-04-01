namespace ScriptBee.Analysis.Instance.Docker.Config;

public class CalculationDockerConfig
{
    public required string DockerSocket { get; init; }

    public string? Network { get; init; }
}
