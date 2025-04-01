using ScriptBee.Analysis.Instance.Docker.Extensions;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Ports.Instance;
using ScriptBee.Web.Config;
using ScriptBee.Web.Exceptions;
using ScriptBee.Web.Services;

namespace ScriptBee.Web.Extensions;

public static class ScriptBeeCalculationConfigExtensions
{
    public static IServiceCollection AddAnalysisConfig(
        this IServiceCollection services,
        ConfigurationManager configurationManager
    )
    {
        var scriptBeeCalculationConfig = configurationManager
            .GetSection("ScriptBee:Calculation")
            .Get<ScriptBeeCalculationConfig>()!;

        if (scriptBeeCalculationConfig.Driver != "Docker")
        {
            throw new AnalysisInstanceDriverTypeNotSupported(scriptBeeCalculationConfig.Driver);
        }

        services.AddSingleton<IInstanceTemplateProvider, InstanceTemplateProvider>(
            _ => new InstanceTemplateProvider(
                new AnalysisInstanceImage(scriptBeeCalculationConfig.Image)
            )
        );

        return services.AddDockerInstanceAdapter("ScriptBee:Calculation:Docker");
    }
}
