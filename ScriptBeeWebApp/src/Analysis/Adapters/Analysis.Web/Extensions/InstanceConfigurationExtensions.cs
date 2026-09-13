using DxWorks.ScriptBee.Analysis.Sdk.Context;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Web.Extensions;

public static class InstanceConfigurationExtensions
{
    public static IServiceCollection AddInstanceConfig(this IServiceCollection services)
    {
        return services
            .AddSingleton<IProjectManager, ProjectManager>(provider =>
            {
                var context = provider.GetRequiredService<IAnalysisInstanceContext>();
                return new ProjectManager(
                    new Project
                    {
                        Id = context.ProjectId.Value,
                        Name = context.ProjectName,
                        CreationDate = DateTimeOffset.UtcNow,
                        Context = new Context(),
                    }
                );
            })
            .AddSingleton<ILoadModelFilesService, LoadModelFilesService>()
            .AddSingleton<IProjectStructureService, ProjectStructureService>();
    }
}
