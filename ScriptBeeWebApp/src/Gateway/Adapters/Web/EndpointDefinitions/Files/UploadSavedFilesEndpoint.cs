using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Adapters.Auth.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Gateway.Files;
using ScriptBee.UseCases.Gateway.Files;
using ScriptBee.Web.EndpointDefinitions.Files.Contracts;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Files;

using UploadResult = Results<Ok<WebUploadSavedFilesResponse>, NotFound<ProblemDetails>>;

public class UploadSavedFilesEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IUploadSavedFilesUseCase, UploadSavedFilesService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/saved-files", UploadSavedFiles)
            .WithTags("Projects", "Files")
            .WithSummary("Upload saved files")
            .WithDescription("Uploads one or more files to the project without requiring a loader.")
            .RequireAction("model:upload");
    }

    private static async Task<UploadResult> UploadSavedFiles(
        HttpContext context,
        [FromRoute] string projectId,
        [FromForm] IFormFileCollection files,
        IUploadSavedFilesUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var uploadFiles = files
            .Select(formFile => new UploadFileInformation(
                formFile.FileName,
                formFile.Length,
                formFile.OpenReadStream()
            ))
            .ToList();

        var command = new UploadSavedFilesCommand(ProjectId.FromValue(projectId), uploadFiles);
        var result = await useCase.Upload(command, cancellationToken);

        return result.Match<UploadResult>(
            fileData =>
                TypedResults.Ok(new WebUploadSavedFilesResponse(fileData.Select(WebFileData.Map))),
            error => error.ToProblem(context)
        );
    }
}
