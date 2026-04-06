using ScriptBee.Analysis.Instance.Docker.Extensions;
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
        ConfigurationManager configurationManager
    )
    {
        var scriptBeeAnalysisConfig = configurationManager
            .GetSection("ScriptBee:Analysis")
            .Get<ScriptBeeAnalysisConfig>()!;

        if (
            !scriptBeeAnalysisConfig.Driver.Equals(
                "Docker",
                StringComparison.InvariantCultureIgnoreCase
            )
        )
        {
            throw new AnalysisInstanceDriverTypeNotSupported(scriptBeeAnalysisConfig.Driver);
        }

        services.AddSingleton<IInstanceTemplateProvider, InstanceTemplateProvider>(
            _ => new InstanceTemplateProvider(
                new AnalysisInstanceImage(scriptBeeAnalysisConfig.Image)
            )
        );

        return services.AddDockerInstanceAdapter("ScriptBee:Analysis:Docker");
    }
}
