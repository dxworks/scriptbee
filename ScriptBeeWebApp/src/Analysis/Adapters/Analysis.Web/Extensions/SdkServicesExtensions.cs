using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Web.Extensions;

public static class SdkServicesExtensions
{
    public static IServiceCollection AddSdkServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IClearContextUseCase, ClearContextService>()
            .AddSingleton<IGenerateClassesUseCase, GenerateClassesService>()
            .AddSingleton<IGetContextUseCase, GetContextService>()
            .AddSingleton<IGetContextGraphUseCase, GetContextGraphService>()
            .AddSingleton<IGetInstalledPluginsUseCase, GetInstalledPluginsService>()
            .AddSingleton<IInstallPluginUseCase, InstallPluginService>()
            .AddSingleton<ILinkContextUseCase, LinkContextService>()
            .AddSingleton<ILoadContextUseCase, LoadContextService>()
            .AddSingleton<IRunAnalysisUseCase, RunAnalysisService>()
            .AddSingleton<IUninstallPluginUseCase, UninstallPluginService>();
    }
}
