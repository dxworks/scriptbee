using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Files;
using ScriptBee.UseCases.Project.Files;
using ScriptBee.Web.EndpointDefinitions.Loaders.Contracts;

namespace ScriptBee.Web.EndpointDefinitions.Loaders;

using UploadResult = Results<Ok<WebUploadLoaderFilesResponse>, NotFound<ProblemDetails>>;

public class UploadLoaderFilesEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IUploadLoaderFilesUseCase, UploadLoaderFilesService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/projects/{projectId}/loaders/{loaderId}/files", UploadLoaderFiles)
            .DisableAntiforgery();
    }

    private static async Task<UploadResult> UploadLoaderFiles(
        HttpContext context,
        [FromRoute] string projectId,
        [FromRoute] string loaderId,
        [FromForm] IFormFileCollection files,
        IUploadLoaderFilesUseCase useCase,
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

        var command = new UploadLoaderFilesCommand(
            ProjectId.FromValue(projectId),
            loaderId,
            uploadFiles
        );
        var result = await useCase.Upload(command, cancellationToken);

        return result.Match<UploadResult>(
            fileData =>
                TypedResults.Ok(
                    new WebUploadLoaderFilesResponse(loaderId, fileData.Select(f => f.Name))
                ),
            error =>
                TypedResults.NotFound(
                    context.ToProblemDetails(
                        "Project Not Found",
                        $"A project with the ID '{error.Id.Value}' does not exists."
                    )
                )
        );
    }
}
