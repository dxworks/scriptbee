using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.Services;
using ScriptBee.Models;

namespace ScriptBee.Services;

public class HelperFunctionsResultService : IHelperFunctionsResultService
{
    private readonly HelperFunctionsSettings _helperFunctionsSettings;
    private readonly IResultCollector _resultCollector;
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;

    public HelperFunctionsResultService(HelperFunctionsSettings helperFunctionsSettings,
        IResultCollector resultCollector, IFileModelService fileModelService, IFileNameGenerator fileNameGenerator)
    {
        _helperFunctionsSettings = helperFunctionsSettings;

        _resultCollector = resultCollector;
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
        var outputFileName = _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
            _helperFunctionsSettings.RunId, type, fileName);

        _resultCollector.Add(outputFileName, type);

        await _fileModelService.UploadFileAsync(outputFileName, content, cancellationToken);
    }

    public void UploadResult(string fileName, string type, string content)
    {
        var byteArray = Encoding.ASCII.GetBytes(content);
        using var stream = new MemoryStream(byteArray);

        UploadResult(fileName, type, stream);
    }

    public void UploadResult(string fileName, string type, Stream content)
    {
        var outputFileName = _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
            _helperFunctionsSettings.RunId, type, fileName);

        _resultCollector.Add(outputFileName, type);

        _fileModelService.UploadFile(outputFileName, content);
    }
}
