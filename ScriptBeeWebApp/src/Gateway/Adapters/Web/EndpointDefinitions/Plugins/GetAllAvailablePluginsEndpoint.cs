using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Service.Project.Plugins;
using ScriptBee.UseCases.Project.Plugins;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class GetAllAvailablePluginsEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetAvailablePluginsUseCase, GetAvailablePluginsService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/plugins", GetAllAvailablePlugins).WithTags("Plugins");
        app.MapGet("/api/plugins/{id}", GetPlugin).WithTags("Plugins");
    }

    private static async Task<Ok<WebAllAvailablePluginsResponse>> GetAllAvailablePlugins(
        HttpContext context,
        IGetAvailablePluginsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var marketPlacePlugins = await useCase.GetMarketPlugins(cancellationToken);

        return TypedResults.Ok(
            new WebAllAvailablePluginsResponse(marketPlacePlugins.Select(WebMarketplacePlugin.Map))
        );
    }

    private static async Task<
        Results<Ok<WebMarketplacePluginWithDetails>, NotFound<ProblemDetails>>
    > GetPlugin(
        HttpContext context,
        [FromRoute] string id,
        IGetAvailablePluginsUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var result = await useCase.GetMarketPlugin(id, cancellationToken);

        return result.Match<Results<Ok<WebMarketplacePluginWithDetails>, NotFound<ProblemDetails>>>(
            plugin => TypedResults.Ok(WebMarketplacePluginWithDetails.Map(plugin)),
            error => error.ToProblem(context)
        );
    }
}
