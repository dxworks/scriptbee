using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Plugin;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class LinkersController : ControllerBase
{
    private readonly IProjectModelService _projectModelService;
    private readonly ILinkersHolder _linkersHolder;
    private readonly IProjectManager _projectManager;

    public LinkersController(IProjectModelService projectModelService, ILinkersHolder linkersHolder,
        IProjectManager projectManager)
    {
        _projectModelService = projectModelService;
        _projectManager = projectManager;
        _linkersHolder = linkersHolder;
    }

    [HttpGet]
    public IActionResult GetAllLinkers()
    {
        return Ok(_linkersHolder.GetAllLinkers().Select(linker => linker.GetName()).ToList());
    }

    [HttpPost]
    // todo extract validation to separate class
    public async Task<IActionResult> Link(LinkProject linkProject, CancellationToken cancellationToken)
    {
        if (linkProject == null || string.IsNullOrWhiteSpace(linkProject.ProjectId) ||
            string.IsNullOrEmpty(linkProject.LinkerName))
        {
            return BadRequest("Invalid arguments. ProjectId and LinkerName needed!");
        }

        var linker = _linkersHolder.GetModelLinker(linkProject.LinkerName);
        if (linker is null)
        {
            return NotFound($"Could not find linker with name: {linkProject.LinkerName}");
        }

        var projectModel = await _projectModelService.GetDocument(linkProject.ProjectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {linkProject.ProjectId}");
        }

        var project = _projectManager.GetProject(linkProject.ProjectId);
        if (project is null)
        {
            return NotFound($"Project with id = {linkProject.ProjectId} does not have its context loaded");
        }

        await linker.LinkModel(project.Context.Models, cancellationToken: cancellationToken);

        projectModel.Linker = linkProject.LinkerName;
        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        return Ok();
    }
}
