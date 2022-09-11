using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Services;

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

    [HttpGet]
    public ActionResult<IEnumerable<PluginManifest>> GetLoadedPlugins()
    {
        return Ok(_pluginService.GetLoadedPlugins());
    }
}
