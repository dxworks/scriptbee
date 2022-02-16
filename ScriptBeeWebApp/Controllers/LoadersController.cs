using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.PluginManager;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class LoadersController : ControllerBase
{
    private readonly ILoadersHolder _loadersHolder;

    public LoadersController(ILoadersHolder loadersHolder)
    {
        _loadersHolder = loadersHolder;
    }

    [HttpGet]
    public IActionResult GetAllProjectLoaders()
    {
        return Ok(_loadersHolder.GetAllLoaders().Select(modelLoader => modelLoader.GetName()).ToList());
    }
}