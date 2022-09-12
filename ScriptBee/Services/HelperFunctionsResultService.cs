using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Models;

namespace ScriptBee.Services;

public class HelperFunctionsResultService : IHelperFunctionsResultService
{
    private readonly HelperFunctionsSettings _helperFunctionsSettings;
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;

    public HelperFunctionsResultService(HelperFunctionsSettings helperFunctionsSettings,
        IFileModelService fileModelService, IFileNameGenerator fileNameGenerator)
    {
        _helperFunctionsSettings = helperFunctionsSettings;
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
    }

    public async Task UploadResultAsync(string fileName, string type, string content,
        CancellationToken cancellationToken = default)
    {
        var byteArray = Encoding.ASCII.GetBytes(content);
        await using var stream = new MemoryStream(byteArray);

        await UploadResultAsync(fileName, type, stream, cancellationToken);
    }

    public async Task UploadResultAsync(string fileName, string type, Stream content,
        CancellationToken cancellationToken = default)
    {
        var outputJsonName = _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
            _helperFunctionsSettings.RunId, type, fileName);

        await _fileModelService.UploadFileAsync(outputJsonName, content, cancellationToken);
    }

    public void UploadResult(string fileName, string type, string content)
    {
        var byteArray = Encoding.ASCII.GetBytes(content);
        using var stream = new MemoryStream(byteArray);

        UploadResult(fileName, type, stream);
    }

    public void UploadResult(string fileName, string type, Stream content)
    {
        var outputJsonName = _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
            _helperFunctionsSettings.RunId, type, fileName);

        _fileModelService.UploadFile(outputJsonName, content);
    }
}
