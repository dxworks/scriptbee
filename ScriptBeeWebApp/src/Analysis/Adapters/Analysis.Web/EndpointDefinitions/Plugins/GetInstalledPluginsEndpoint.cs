using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Common.Web;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins;

public class GetInstalledPluginsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetInstalledPluginsUseCase, GetInstalledPluginsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/plugins", GetInstalledPlugins).WithTags("Plugins");
    }

    private static Ok<WebGetInstalledPluginsResponse> GetInstalledPlugins(
        IGetInstalledPluginsUseCase useCase
    )
    {
        var plugins = useCase.Get();

        return TypedResults.Ok(
            new WebGetInstalledPluginsResponse(plugins.Select(WebInstalledPlugin.Map))
        );
    }
}
