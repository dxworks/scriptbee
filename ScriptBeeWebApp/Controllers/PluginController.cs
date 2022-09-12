using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class PluginController : ControllerBase
{
    private readonly IPluginService _pluginService;

    public PluginController(IPluginService pluginService)
    {
        _pluginService = pluginService;
    }

    // todo allow subclasses to be returned and serialized
    [HttpGet]
    public ActionResult<IEnumerable<PluginManifest>> GetLoadedPlugins([FromQuery] string? type = null)
    {
        if (string.IsNullOrEmpty(type))
        {
            return Ok(_pluginService.GetLoadedPlugins());
        }

        return Ok(
            _pluginService.GetLoadedPlugins()
                .Where(manifest => manifest.Kind == type));
    }

    // todo temporary workaround until above todo is fixed
    [HttpGet("ui")]
    public ActionResult<IEnumerable<UiPluginManifest>> GetLoadedUiPlugins()
    {
        return Ok(_pluginService.GetLoadedPlugins()
            .OfType<UiPluginManifest>());
    }
}
