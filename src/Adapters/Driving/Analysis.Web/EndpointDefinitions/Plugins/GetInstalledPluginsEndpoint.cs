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
        app.MapGet("/api/plugins", GetInstalledPlugins);
    }

    private static async Task<Ok<IEnumerable<WebInstalledPlugin>>> GetInstalledPlugins(
        IGetInstalledPluginsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var plugins = await useCase.Get(cancellationToken);

        return TypedResults.Ok(plugins.Select(WebInstalledPlugin.Map));
    }
}
