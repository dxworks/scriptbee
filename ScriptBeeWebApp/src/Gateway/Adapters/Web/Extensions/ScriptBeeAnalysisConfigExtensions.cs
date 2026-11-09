using ScriptBee.Analysis.Instance.Docker.Extensions;
using ScriptBee.Analysis.Instance.Kubernetes.Extensions;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Ports.Instance;
using ScriptBee.Web.Config;
using ScriptBee.Web.Exceptions;
using ScriptBee.Web.Services;

namespace ScriptBee.Web.Extensions;

public static class ScriptBeeAnalysisConfigExtensions
{
    public static IServiceCollection AddAnalysisConfig(
        this IServiceCollection services,
        string analysisConfiguration,
        ConfigurationManager configurationManager
    )
    {
        var scriptBeeAnalysisConfig = configurationManager
            .GetSection(analysisConfiguration)
            .Get<ScriptBeeAnalysisConfig>()!;

        services.AddSingleton<IInstanceTemplateProvider, InstanceTemplateProvider>(
            _ => new InstanceTemplateProvider(
                new AnalysisInstanceImage(scriptBeeAnalysisConfig.Image)
            )
        );

        if (
            scriptBeeAnalysisConfig.Driver.Equals(
                nameof(scriptBeeAnalysisConfig.Docker),
                StringComparison.InvariantCultureIgnoreCase
            )
        )
        {
            return services.AddDockerInstanceAdapter(
                $"{analysisConfiguration}:{nameof(scriptBeeAnalysisConfig.Docker)}"
            );
        }

        if (
            scriptBeeAnalysisConfig.Driver.Equals(
                nameof(scriptBeeAnalysisConfig.Kubernetes),
                StringComparison.InvariantCultureIgnoreCase
            )
        )
        {
            return services.AddKubernetesInstanceAdapter(
                $"{analysisConfiguration}:{nameof(scriptBeeAnalysisConfig.Kubernetes)}"
            );
        }

        throw new AnalysisInstanceDriverTypeNotSupported(scriptBeeAnalysisConfig.Driver);
    }
}
