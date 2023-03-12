using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.ProjectContext;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.Repository;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

// todo pact add tests
public class LinkersEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/linkers", GetAllLinkers);
        app.MapPost("/api/linkers", Link);
    }

    public static IEnumerable<string> GetAllLinkers(ILinkersService linkersService)
    {
        return linkersService.GetSupportedLinkers();
    }

    public static async Task<IResult> Link([FromBody] LinkProject linkProject, IValidator<LinkProject> linkProjectValidator,
        ILinkersService linkersService, IProjectModelService projectModelService, IProjectManager projectManager,
        CancellationToken cancellationToken)
    {
        var validationResult = await linkProjectValidator.ValidateAsync(linkProject, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var linker = linkersService.GetLinker(linkProject.LinkerName);
        if (linker is null)
        {
            return Results.NotFound($"Could not find linker with name: {linkProject.LinkerName}");
        }

        var projectModel = await projectModelService.GetDocument(linkProject.ProjectId, cancellationToken);
        if (projectModel == null)
        {
            return Results.NotFound($"Could not find project model with id: {linkProject.ProjectId}");
        }

        var project = projectManager.GetProject(linkProject.ProjectId);
        if (project is null)
        {
            return Results.NotFound($"Project with id = {linkProject.ProjectId} does not have its context loaded");
        }

        await linker.LinkModel(project.Context.Models, cancellationToken: cancellationToken);

        projectModel.Linker = linkProject.LinkerName;
        await projectModelService.UpdateDocument(projectModel, cancellationToken);

        // todo return something like the current state of the project with the linker applied
        return Results.Ok("");
    }
}
