using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Web.Extensions;

public static class ProjectContextConfigExtensions
{
    public static IServiceCollection AddProjectContextConfig(this IServiceCollection services)
    {
        return services
            .AddSingleton<IProjectManager, ProjectManager>()
            .AddSingleton<ILoadModelFilesService, LoadModelFilesService>();
    }
}
