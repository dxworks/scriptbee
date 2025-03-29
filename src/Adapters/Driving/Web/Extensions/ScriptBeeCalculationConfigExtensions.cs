using ScriptBee.Analysis.Instance.Docker;
using ScriptBee.Analysis.Instance.Docker.Config;
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

        services
            .AddOptions<CalculationDockerConfig>()
            .BindConfiguration("ScriptBee:Calculation:Docker");

        return services
            .AddSingleton<IAllocateInstance, CalculationInstanceDockerAdapter>()
            .AddSingleton<IDeallocateInstance, CalculationInstanceDockerAdapter>();
    }
}
