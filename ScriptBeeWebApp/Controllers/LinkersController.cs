using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class LinkersController : ControllerBase
{
    private readonly IProjectModelService _projectModelService;
    private readonly ILinkersService _linkersService;
    private readonly IProjectManager _projectManager;
    private readonly IValidator<LinkProject> _linkProjectValidator;

    public LinkersController(IProjectModelService projectModelService, ILinkersService linkersService,
        IProjectManager projectManager, IValidator<LinkProject> linkProjectValidator)
    {
        _projectModelService = projectModelService;
        _projectManager = projectManager;
        _linkProjectValidator = linkProjectValidator;
        _linkersService = linkersService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<string>> GetAllLinkers()
    {
        return Ok(_linkersService.GetSupportedLinkers());
    }

    [HttpPost]
    public async Task<IActionResult> Link(LinkProject linkProject, CancellationToken cancellationToken)
    {
        var validationResult = await _linkProjectValidator.ValidateAsync(linkProject, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var linker = _linkersService.GetLinker(linkProject.LinkerName);
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

        // todo return something like the current state of the project with the linker applied
        return Ok("");
    }
}
