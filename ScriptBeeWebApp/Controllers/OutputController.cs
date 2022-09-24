using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Services;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
// todo add tests
public class OutputController : ControllerBase
{
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IRunModelService _runModelService;

    public OutputController(IFileModelService fileModelService, IFileNameGenerator fileNameGenerator,
        IRunModelService runModelService)
    {
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
        _runModelService = runModelService;
    }

    [HttpGet("{outputId}")]
    public async Task<ActionResult<string>> GetOutput([FromRoute] string outputId)
    {
        await using var outputStream = await _fileModelService.GetFileAsync(outputId);
        using var streamReader = new StreamReader(outputStream);

        var outputContent = await streamReader.ReadToEndAsync();
        return Ok(outputContent);
    }


    [HttpGet("console")]
    public async Task<IActionResult> GetConsoleOutputContent([FromQuery] string consoleOutputPath)
    {
        if (string.IsNullOrEmpty(consoleOutputPath))
        {
            return BadRequest("You must provide a consoleOutputPath for this operation");
        }

        await using var outputStream = await _fileModelService.GetFileAsync(consoleOutputPath);
        using StreamReader streamReader = new StreamReader(outputStream);

        var consoleOutputContent = await streamReader.ReadToEndAsync();
        return Ok(consoleOutputContent);
    }

    [HttpPost("files/download"), DisableRequestSizeLimit]
    // todo extract validation to separate class
    public async Task<IActionResult> DownloadFile(DownloadFile downloadFile)
    {
        if (downloadFile is null || string.IsNullOrEmpty(downloadFile.FilePath))
        {
            return BadRequest("You must provide a download file path for this operation");
        }

        var outputStream = await _fileModelService.GetFileAsync(downloadFile.FilePath);
        const string contentType = "application/octet-stream";
        var (_, _, _, fileName) = _fileNameGenerator.ExtractOutputFileNameComponents(downloadFile.FilePath);

        return File(outputStream, contentType, fileName);
    }

    [HttpPost("files/downloadAll")]
    // todo extract validation to separate class
    public async Task<IActionResult> DownloadFile(DownloadAll downloadAll,
        CancellationToken cancellationToken)
    {
        if (downloadAll is null || string.IsNullOrEmpty(downloadAll.ProjectId) ||
            string.IsNullOrEmpty(downloadAll.RunId))
        {
            return BadRequest("You must provide a projectId and a runId for this operation");
        }

        var runModel = await _runModelService.GetDocument(downloadAll.RunId, cancellationToken);
        if (runModel == null)
        {
            return NotFound("The given runId does not correspond to a valid run");
        }

        List<OutputFileStream> files = new();

        foreach (var outputFilePath in runModel.OutputFileNames)
        {
            var (_, _, _, fileName) = _fileNameGenerator.ExtractOutputFileNameComponents(outputFilePath);
            var outputStream = await _fileModelService.GetFileAsync(outputFilePath);
            files.Add(new OutputFileStream(fileName, outputStream));
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
