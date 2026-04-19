using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Artifacts.Extensions;

public static class ConfigureProjectScriptingExtensions
{
    public static IServiceCollection AddArtifactFileAdapters(this IServiceCollection services)
    {
        return services
            .AddSingleton<IConfigFoldersService, ConfigFoldersService>()
            .AddSingleton<ICreateFile, CreateFileAdapter>()
            .AddSingleton<ILoadFile, LoadFileAdapter>()
            .AddSingleton<IUpdateFile, UpdateFileAdapter>()
            .AddSingleton<IDeleteFileOrFolder, DeleteFileOrFolderAdapter>();
    }
}
