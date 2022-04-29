using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class LinkersController : ControllerBase
{
    private readonly IProjectModelService _projectModelService;

    public LinkersController(IProjectModelService projectModelService)
    {
        _projectModelService = projectModelService;
    }

    [HttpPost]
    public async Task<IActionResult> Link(LinkProject linkProject, CancellationToken cancellationToken)
    {
        if (linkProject == null || string.IsNullOrWhiteSpace(linkProject.ProjectId) ||
            string.IsNullOrEmpty(linkProject.LinkerName))
        {
            return BadRequest("Invalid arguments. ProjectId needed!");
        }

        var projectModel = await _projectModelService.GetDocument(linkProject.ProjectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {linkProject.ProjectId}");
        }

        projectModel.Linker = linkProject.LinkerName;
        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        return Ok();
    }
}