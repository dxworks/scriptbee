using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Plugin;
using ScriptBee.Plugin.Manifest;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
// todo add tests
public class PluginController : ControllerBase
{
    private readonly IPluginRepository _pluginRepository;

    public PluginController(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    // todo allow subclasses to be returned and serialized
    [HttpGet]
    public ActionResult<IEnumerable<PluginManifest>> GetLoadedPlugins([FromQuery] string? type = null)
    {
        if (string.IsNullOrEmpty(type))
        {
            return Ok(_pluginRepository.GetLoadedPluginManifests<PluginManifest>());
        }

        return Ok(
            _pluginRepository.GetLoadedPluginManifests<PluginManifest>()
                .Where(manifest => manifest.Kind == type));
    }

    // todo temporary workaround until above todo is fixed
    [HttpGet("ui")]
    public ActionResult<IEnumerable<UiPluginManifest>> GetLoadedUiPlugins()
    {
        return Ok(_pluginRepository.GetLoadedPluginManifests<UiPluginManifest>());
    }
}
