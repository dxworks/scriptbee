using ScriptBee.Domain.Model.Instance;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Web.Extensions;

public static class InstanceConfigurationExtensions
{
    public static IServiceCollection AddInstanceConfig(
        this IServiceCollection services,
        IConfigurationSection scriptBeeConfigurationSection
    )
    {
        var instanceId =
            scriptBeeConfigurationSection.GetValue<string>("InstanceId") ?? "no-name-instances";
        return services.AddSingleton<InstanceInformation>(_ => new InstanceInformation
        {
            Id = new InstanceId(instanceId),
        });
    }
}
