using ScriptBee.Domain.Model.Context;
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
        var projectId =
            scriptBeeConfigurationSection.GetValue<string>("ProjectId") ?? "no-project-id";
        var projectName =
            scriptBeeConfigurationSection.GetValue<string>("ProjectName") ?? "no-project-name";

        return services
            .AddSingleton<InstanceInformation>(_ => new InstanceInformation
            {
                Id = new InstanceId(instanceId),
            })
            .AddSingleton<IProjectManager, ProjectManager>(_ => new ProjectManager(
                new Project
                {
                    Id = projectId,
                    Name = projectName,
                    CreationDate = DateTimeOffset.UtcNow,
                    Context = new Context(),
                }
            ))
            .AddSingleton<ILoadModelFilesService, LoadModelFilesService>()
            .AddSingleton<IProjectStructureService, ProjectStructureService>();
    }
}
