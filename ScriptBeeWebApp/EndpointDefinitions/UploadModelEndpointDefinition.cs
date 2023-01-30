using Microsoft.AspNetCore.Mvc;
using ScriptBeeWebApp.EndpointDefinitions.DTO;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

// todo pact add tests
public class UploadModelEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("/api/uploadmodel/fromfile", UploadFromFile);
    }


    [DisableRequestSizeLimit]
    public static async Task<IResult> UploadFromFile(HttpRequest request,
        IProjectModelService projectModelService, IUploadModelService uploadModelService,
        CancellationToken cancellationToken = default)
    {
        var formData = request.Form;

        if (!formData.TryGetValue("loaderName", out var loaderName))
        {
            return Results.BadRequest("Missing loader name");
        }

        if (!formData.TryGetValue("projectId", out var projectId))
        {
            return Results.BadRequest("Missing project id");
        }

        var projectModel = await projectModelService.GetDocument(projectId, cancellationToken);
        if (projectModel == null)
        {
            return Results.NotFound($"Could not find project model with id: {projectId}");
        }

        var fileData =
            await uploadModelService.UploadFilesAsync(projectModel, loaderName, formData.Files, cancellationToken);

        var fileNames = fileData.Select(d => d.Name).ToList();

        return Results.Ok(new ReturnedNode(loaderName, fileNames));
    }
}
