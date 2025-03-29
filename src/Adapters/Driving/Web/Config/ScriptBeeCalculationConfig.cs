using ScriptBee.Analysis.Instance.Docker.Config;

namespace ScriptBee.Web.Config;

public class ScriptBeeCalculationConfig
{
    public required string Image { get; init; }

    public required string Driver { get; init; }

    public CalculationDockerConfig? Docker { get; init; }
}
