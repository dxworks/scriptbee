using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptBeeWebApp.Controllers.Arguments;
using ScriptBeeWebApp.Services;

namespace ScriptBeeWebApp.Controllers;

[ApiControllerRoute]
[ApiController]
public class OutputController : ControllerBase
{
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;

    public OutputController(IFileModelService fileModelService, IFileNameGenerator fileNameGenerator)
    {
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
    }

    [HttpGet("console")]
    public async Task<IActionResult> GetConsoleOutputContent([FromQuery] string consoleOutputPath)
    {
        if (string.IsNullOrEmpty(consoleOutputPath))
        {
            return BadRequest("You must provide a consoleOutputPath for this operation");
        }

        await using var outputStream = await _fileModelService.GetFile(consoleOutputPath);
        using StreamReader streamReader = new StreamReader(outputStream);

        var consoleOutputContent = await streamReader.ReadToEndAsync();
        return Ok(consoleOutputContent);
    }

    [HttpPost("files/download"), DisableRequestSizeLimit]
    public async Task<IActionResult> DownloadFile(DownloadFile downloadFile)
    {
        if (downloadFile is null || string.IsNullOrEmpty(downloadFile.FilePath))
        {
            return BadRequest("You must provide a download file path for this operation");
        }

        var outputStream = await _fileModelService.GetFile(downloadFile.FilePath);
        const string contentType = "application/octet-stream";
        var (_, _, _, fileName) = _fileNameGenerator.ExtractOutputFileNameComponents(downloadFile.FilePath);

        return File(outputStream, contentType, fileName);
    }

    [HttpPost("files/downloadAll")]
    public async Task<IActionResult> DownloadFile(DownloadAll downloadAll,
        CancellationToken cancellationToken)
    {
        return Ok();
    }
}