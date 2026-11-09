using ScriptBee.Analysis.Instance.Docker.Config;
using ScriptBee.Analysis.Instance.Kubernetes.Config;

namespace ScriptBee.Web.Config;

public class ScriptBeeAnalysisConfig
{
    public required string Image { get; init; }

    public required string Driver { get; init; }

    public AnalysisDockerConfig? Docker { get; init; }

    public AnalysisKubernetesConfig? Kubernetes { get; init; }
}
