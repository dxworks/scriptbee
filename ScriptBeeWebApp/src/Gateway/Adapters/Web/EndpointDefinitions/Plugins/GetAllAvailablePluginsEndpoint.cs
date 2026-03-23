using Microsoft.AspNetCore.Http.HttpResults;
using ScriptBee.Common.Web;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Plugins;

public class GetAllAvailablePluginsEndpoint : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        // TODO FIXIT(#87): replace hardcoded values with use cases
        app.MapGet("/api/plugins", GetAllAvailablePlugins);
        app.MapGet("/api/plugins/{id}", GetPlugin);
    }

    private static readonly WebMarketplacePluginWithDetails[] MockPlugins = CreateMockData();

    private static WebMarketplacePluginWithDetails[] CreateMockData()
    {
        var now = DateTime.UtcNow;
        return
        [
            new WebMarketplacePluginWithDetails(
                "plugin1",
                "Python Plugin",
                WebMarketplacePlugin.PluginType,
                "Allows running Python scripts for analysis and automation.",
                ["John Doe", "Jane Smith"],
                [
                    new WebPluginVersion("1.0.0", false, now.AddMonths(-6)),
                    new WebPluginVersion("1.1.0", true, now.AddMonths(-3)),
                    new WebPluginVersion("1.2.0", false, now.AddDays(-10)),
                ],
                null,
                "https://github.com/dxworks/scriptbee-python",
                "https://raw.github.../manifest.yaml",
                "https://scriptbee.com/python",
                "MIT",
                ["Language", "Python", "Automation"],
                ["Python", "IronPython"],
                [
                    new WebExtensionPoint("ScriptRunner", "python", ".py"),
                    new WebExtensionPoint("ScriptGenerator", "python", ".py"),
                ]
            ),
            new WebMarketplacePluginWithDetails(
                "plugin2",
                "Javascript Plugin",
                WebMarketplacePlugin.PluginType,
                "Enables Javascript execution for web-related tasks.",
                ["Alice Wonderland"],
                [
                    new WebPluginVersion("2.0.1", false, now.AddMonths(-2)),
                    new WebPluginVersion("2.1.0", false, now.AddDays(-5)),
                ],
                null,
                null,
                null,
                null,
                null,
                ["Language", "Javascript", "Web"],
                ["Jint"],
                [new WebExtensionPoint("ScriptRunner", "javascript", ".js")]
            ),
            new WebMarketplacePluginWithDetails(
                "bundle1",
                "Standard Bundle",
                WebMarketplacePlugin.BundleType,
                "A collection of standard plugins for general use.",
                ["ScriptBee Team"],
                [new WebPluginVersion("1.0.0", true, now.AddMonths(-1))],
                [
                    new WebBundleItem("plugin1", "Python Plugin"),
                    new WebBundleItem("plugin2", "Javascript Plugin"),
                ],
                "https://github.com/dxworks/scriptbee-bundle",
                null,
                "https://github.com/dxworks/scriptbee",
                "Apache-2.0",
                ["Helper Functions", "Script Generation"],
                ["C#", "Javascript", "Python"]
            ),
            new WebMarketplacePluginWithDetails(
                "plugin3",
                "C# Scripting",
                WebMarketplacePlugin.PluginType,
                "Run C# scripts directly in your project.",
                ["Bob Builder"],
                [
                    new WebPluginVersion("1.0.0", false, now.AddMonths(-1)),
                    new WebPluginVersion("1.0.1", false, now.AddDays(-2)),
                ]
            ),
        ];
    }

    private static Task<Ok<WebAllAvailablePluginsResponse>> GetAllAvailablePlugins(
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        var basicList = MockPlugins
            .Select(p => new WebMarketplacePlugin(
                p.Id,
                p.Name,
                p.Type,
                p.Description,
                p.Authors,
                p.Versions.MaxBy(v => v.Version)?.Version,
                p.Versions.FirstOrDefault(v => v.Installed)?.Version
            ))
            .ToArray();

        var response = new WebAllAvailablePluginsResponse(basicList);
        return Task.FromResult(TypedResults.Ok(response));
    }

    private static Task<Results<Ok<WebMarketplacePluginWithDetails>, NotFound>> GetPlugin(
        string id,
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        var plugin = MockPlugins.FirstOrDefault(p => p.Id == id);
        if (plugin is null)
        {
            return Task.FromResult<Results<Ok<WebMarketplacePluginWithDetails>, NotFound>>(
                TypedResults.NotFound()
            );
        }

        return Task.FromResult<Results<Ok<WebMarketplacePluginWithDetails>, NotFound>>(
            TypedResults.Ok(plugin)
        );
    }
}
