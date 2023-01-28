using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBee.Services.Config;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Extensions;

public static class ControllerServicesExtensions
{
    public static IServiceCollection AddControllerServices(this IServiceCollection services,  IConfigurationSection userFolderConfigurationSection)
    {
        // todo: move in endpoint definition
        services.Configure<UserFolderSettings>(userFolderConfigurationSection);

        services.AddSingleton<IProjectManager, ProjectManager>();
        services.AddSingleton<IProjectFileStructureManager, ProjectFileStructureManager>();
        services.AddSingleton<IProjectStructureService, ProjectStructureService>();
        services.AddSingleton<IProjectModelService, ProjectModelService>();
        services.AddSingleton<IRunModelService, RunModelService>();
        services.AddSingleton<IUploadModelService, UploadModelService>();
        services.AddSingleton<ILoadersService, LoadersService>();
        services.AddSingleton<ILinkersService, LinkersService>();
        services.AddSingleton<IGenerateScriptService, GenerateScriptService>();
        services.AddSingleton<IFileModelService, FileModelService>();
        services.AddSingleton<IRunScriptService, RunScriptService>();
        
        return services;
    }
}
