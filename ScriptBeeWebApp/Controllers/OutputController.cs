using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.Services;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Services;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
// todo pact add tests
public class OutputController : ControllerBase
{
    private readonly IFileModelService _fileModelService;
    private readonly IRunModelService _runModelService;

    public OutputController(IFileModelService fileModelService, IRunModelService runModelService)
    {
        _fileModelService = fileModelService;
        _runModelService = runModelService;
    }

    // todo manual test

    [HttpGet("{outputId}")]
    public async Task<ActionResult<string>> GetOutput([FromRoute] string outputId)
    {
        await using var outputStream = await _fileModelService.GetFileAsync(outputId);
        using var streamReader = new StreamReader(outputStream);

        var outputContent = await streamReader.ReadToEndAsync();
        return Ok(outputContent);
    }


    // todo to be removed
    [HttpGet("console")]
    public async Task<IActionResult> GetConsoleOutputContent([FromQuery] string consoleOutputPath)
    {
        if (string.IsNullOrEmpty(consoleOutputPath))
        {
            return BadRequest("You must provide a consoleOutputPath for this operation");
        }

        await using var outputStream = await _fileModelService.GetFileAsync(consoleOutputPath);
        using var streamReader = new StreamReader(outputStream);

        var consoleOutputContent = await streamReader.ReadToEndAsync();
        return Ok(consoleOutputContent);
    }

    [HttpPost("files/download")]
    [DisableRequestSizeLimit]
    // todo extract validation to separate class
    public async Task<IActionResult> DownloadFile(DownloadFile downloadFile)
    {
        if (downloadFile is null)
        {
            return BadRequest("You must provide a download file path for this operation");
        }

        var outputStream = await _fileModelService.GetFileAsync(downloadFile.Id.ToString());

        return File(outputStream, "application/octet-stream", downloadFile.Name);
    }

    [HttpPost("files/downloadAll")]
    // todo extract validation to separate class
    public async Task<IActionResult> DownloadFile(DownloadAll downloadAll,
        CancellationToken cancellationToken)
    {
        if (downloadAll is null || string.IsNullOrEmpty(downloadAll.ProjectId))
        {
            return BadRequest("You must provide a projectId and a runId for this operation");
        }

        var runModel = await _runModelService.GetDocument(downloadAll.ProjectId, cancellationToken);
        if (runModel == null)
        {
            return NotFound("The given runId does not correspond to a valid run");
        }

        List<OutputFileStream> files = new();

        foreach (var runResult in runModel.Runs[downloadAll.RunIndex].Results
                     .Where(r => r.Type == RunResultDefaultTypes.FileType))
        {
            var outputStream = await _fileModelService.GetFileAsync(runResult.Id.ToString());
            files.Add(new OutputFileStream(runResult.Name, outputStream));
        }

        var zipStream = CreateFilesZipStream(files);

        return File(zipStream, "application/octet-stream", "outputFiles.zip");
    }

    private Stream CreateFilesZipStream(IEnumerable<OutputFileStream> outputFiles)
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

    private record OutputFileStream(string Name, Stream Content);
}
