using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBeeWebApp.Data;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class TempController : ControllerBase
{
    private readonly IFileWatcherHubService _fileWatcherHubService;

    public TempController(IFileWatcherHubService fileWatcherHubService)
    {
        _fileWatcherHubService = fileWatcherHubService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var watchFile = new WatchedFile("path", "content");

        await _fileWatcherHubService.SendFileWatch(watchFile);

        return Ok(watchFile);
    }
}
