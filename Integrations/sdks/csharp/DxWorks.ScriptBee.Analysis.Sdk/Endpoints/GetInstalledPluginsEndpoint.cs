using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using ScriptBee.Common.Web;

namespace DxWorks.ScriptBee.Analysis.Sdk.Endpoints;

public class GetInstalledPluginsEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/plugins", GetInstalledPlugins)
            .WithTags("Plugins")
            .WithSummary("Get installed plugins")
            .WithDescription(
                "Retrieves a list of all plugins currently installed in the analysis service."
            )
            .WithName("PluginsGet");
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
