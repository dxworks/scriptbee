using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Service.Gateway.Plugins;
using ScriptBee.UseCases.Gateway.Plugins;
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
        app.MapGet("/api/plugins", GetAllAvailablePlugins)
            .WithTags("Plugins")
            .WithSummary("Get all available plugins")
            .WithDescription("Retrieves a list of all available plugins from the marketplace.");
        app.MapGet("/api/plugins/{id}", GetPlugin)
            .WithTags("Plugins")
            .WithSummary("Get plugin by ID")
            .WithDescription(
                "Retrieves detailed information about a specific plugin from the marketplace by its unique identifier."
            );
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
