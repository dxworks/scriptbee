using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
// todo add tests
public class UploadModelController : ControllerBase
{
    private readonly IProjectModelService _projectModelService;
    private readonly IUploadModelService _uploadModelService;

    public UploadModelController(IProjectModelService projectModelService, IUploadModelService uploadModelService)
    {
        _projectModelService = projectModelService;
        _uploadModelService = uploadModelService;
    }

    [HttpPost("fromfile")]
    public async Task<IActionResult> UploadFromFile(IFormCollection formData,
        CancellationToken cancellationToken = default)
    {
        if (!formData.TryGetValue("loaderName", out var loaderName))
        {
            return BadRequest("Missing loader name");
        }

        if (!formData.TryGetValue("projectId", out var projectId))
        {
            return BadRequest("Missing project id");
        }

        var projectModel = await _projectModelService.GetDocument(projectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {projectId}");
        }

        var fileData =
            await _uploadModelService.UploadFilesAsync(projectModel, loaderName, formData.Files, cancellationToken);

        var fileNames = fileData.Select(d => d.Name).ToList();

        return Ok(new ReturnedNode(loaderName, fileNames));
    }
}
