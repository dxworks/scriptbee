using System.IO.Compression;
using DxWorks.ScriptBee.Plugin.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Services;
using ScriptBeeWebApp.EndpointDefinitions.Arguments;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.EndpointDefinitions;

// todo pact add tests
public class OutputEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        //
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/output/{outputId}", GetOutput);
        app.MapPost("/api/output/files/download", DownloadFile);
        app.MapPost("/api/output/files/downloadAll", DownloadAllFiles);
    }

    public static async Task<IResult> GetOutput([FromRoute] string outputId, IFileModelService fileModelService)
    {
        await using var outputStream = await fileModelService.GetFileAsync(outputId);
        using var streamReader = new StreamReader(outputStream);

        var outputContent = await streamReader.ReadToEndAsync();
        return Results.Ok(outputContent);
    }

    [HttpPost("files/download")]
    [DisableRequestSizeLimit]
    // todo extract validation to separate class
    public static async Task<IResult> DownloadFile([FromBody] DownloadFile downloadFile,
        IValidator<DownloadFile> validator, IFileModelService fileModelService)
    {
        var validationResult = await validator.ValidateAsync(downloadFile);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var outputStream = await fileModelService.GetFileAsync(downloadFile.Id.ToString());

        return Results.File(outputStream, "application/octet-stream", downloadFile.Name);
    }

    public static async Task<IResult> DownloadAllFiles([FromBody] DownloadAll downloadAll,
        IValidator<DownloadAll> validator, IFileModelService fileModelService, IRunModelService runModelService,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(downloadAll, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.GetValidationErrorsResponse());
        }

        var runModel = await runModelService.GetDocument(downloadAll.ProjectId, cancellationToken);
        if (runModel == null)
        {
            return Results.NotFound("The given runId does not correspond to a valid run");
        }

        List<OutputFileStream> files = new();

        foreach (var runResult in runModel.Runs[downloadAll.RunIndex].Results
                     .Where(r => r.Type == RunResultDefaultTypes.FileType))
        {
            var outputStream = await fileModelService.GetFileAsync(runResult.Id.ToString());
            files.Add(new OutputFileStream(runResult.Name, outputStream));
        }

        var zipStream = CreateFilesZipStream(files);

        return Results.File(zipStream, "application/octet-stream", "outputFiles.zip");
    }

    private static Stream CreateFilesZipStream(IEnumerable<OutputFileStream> outputFiles)
    {
        var zipStream = new MemoryStream();
        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var (name, content) in outputFiles)
            {
                var zipArchiveEntry = zip.CreateEntry(name);

                using var destinationStream = zipArchiveEntry.Open();

                var buffer = new byte[1024];
                int len;
                while ((len = content.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destinationStream.Write(buffer, 0, len);
                }
            }
        }

        zipStream.Position = 0;
        return zipStream;
    }

    private sealed record OutputFileStream(string Name, Stream Content);
}
