using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Services;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class UploadModelController : ControllerBase
{
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IFileModelService _fileModelService;
    private readonly IProjectModelService _projectModelService;

    public UploadModelController(IFileNameGenerator fileNameGenerator, IFileModelService fileModelService,
        IProjectModelService projectModelService)
    {
        _fileNameGenerator = fileNameGenerator;
        _fileModelService = fileModelService;
        _projectModelService = projectModelService;
    }

    [HttpPost("fromfile")]
    public async Task<IActionResult> UploadFromFile(IFormCollection formData, CancellationToken cancellationToken)
    {
        if (!formData.TryGetValue("loaderName", out var loaderName))
        {
            return BadRequest("Missing loader name");
        }

        if (!formData.TryGetValue("projectId", out var projectId))
        {
            return BadRequest("Missing project id");
        }

        var savedFiles = new List<string>();

        foreach (var file in formData.Files)
        {
            if (file.Length > 0)
            {
                var modelName = _fileNameGenerator.GenerateModelName(projectId, loaderName, file.FileName);

                savedFiles.Add(modelName);

                await using var stream = file.OpenReadStream();
                await _fileModelService.UploadFileAsync(modelName, stream, cancellationToken);
            }
        }

        var projectModel = await _projectModelService.GetDocument(projectId, cancellationToken);
        if (projectModel == null)
        {
            return NotFound($"Could not find project model with id: {projectId}");
        }

        if (projectModel.SavedFiles.TryGetValue(loaderName, out var previousSavedFiles))
        {
            foreach (var previousSavedFile in previousSavedFiles)
            {
                await _fileModelService.DeleteFileAsync(previousSavedFile, cancellationToken);
            }
        }

        projectModel.SavedFiles[loaderName] = savedFiles;

        await _projectModelService.UpdateDocument(projectModel, cancellationToken);

        return Ok();
    }
}
