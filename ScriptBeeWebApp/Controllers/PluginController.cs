using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
// todo add tests
public class PluginController : ControllerBase
{
    private readonly IPluginService _pluginService;


    // todo allow subclasses to be returned and serialized
    public PluginController(IPluginService pluginService)
    {
        _pluginService = pluginService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PluginManifest>> GetLoadedPlugins([FromQuery] string? type = null)
    {
        if (string.IsNullOrEmpty(type))
        {
            return Ok(_pluginService.GetPluginManifests());
        }

        return Ok(_pluginService.GetPluginManifests(type));
    }

    // todo temporary workaround until above todo is fixed
    [HttpGet("ui")]
    public ActionResult<IEnumerable<UiPluginExtensionPoint>> GetLoadedUiPlugins()
    {
        return Ok(_pluginService.GetExtensionPoints<UiPluginExtensionPoint>());
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<MarketplacePlugin>>> GetMarketPlugins([FromQuery] int start = 0,
        [FromQuery] int count = 10, CancellationToken cancellationToken = default)
    {
        var baseMarketplacePlugins = await _pluginService.GetMarketPlugins(start, count, cancellationToken);

        baseMarketplacePlugins = baseMarketplacePlugins
            .Select(plugin =>
            {
                var versions = plugin.Versions.OrderByDescending(version => version.Version);
                return plugin with { Versions = versions.ToList() };
            })
            .Skip(start)
            .Take(count);
        return Ok(baseMarketplacePlugins);
    }

    [HttpPost("install")]
    public async Task<ActionResult> InstallPlugin([FromBody] InstallPluginRequest request,
        CancellationToken cancellationToken = default)
    {
        await _pluginService.InstallPlugin(request.PluginId, request.Version, cancellationToken);
        return Ok();
    }

    [HttpDelete("uninstall/{pluginId}/{pluginVersion}")]
    public ActionResult UninstallPlugin(string pluginId, string pluginVersion)
    {
        _pluginService.UninstallPlugin(pluginId, pluginVersion);
        return Ok();
    }
}
