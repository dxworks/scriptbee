using System.Collections.Generic;
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
    public ActionResult<IEnumerable<BaseMarketplacePlugin>> GetMarketPlugins([FromQuery] int start = 0, [FromQuery] int count = 10)
    {
        return Ok(_pluginService.GetMarketPlugins(start, count));
    }

    [HttpPost("install")]
    public async Task<ActionResult> InstallPlugin([FromBody] InstallPluginRequest request)
    {
        await _pluginService.InstallPlugin(request.PluginId, request.DownloadUrl);
        return Ok();
    }

    [HttpDelete("uninstall/{pluginId}")]
    public async Task<ActionResult> UninstallPlugin(string pluginId)
    {
        await _pluginService.UninstallPlugin(pluginId);
        return Ok();
    }
}
