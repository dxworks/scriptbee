using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.Services;
using ScriptBee.Models;

namespace ScriptBee.Services;

// todo add tests
public class HelperFunctionsResultService : IHelperFunctionsResultService
{
    private readonly HelperFunctionsSettings _helperFunctionsSettings;
    private readonly IResultCollector _resultCollector;
    private readonly IFileModelService _fileModelService;
    private readonly IGuidGenerator _guidGenerator;

    public HelperFunctionsResultService(HelperFunctionsSettings helperFunctionsSettings,
        IResultCollector resultCollector, IFileModelService fileModelService, IGuidGenerator guidGenerator)
    {
        _helperFunctionsSettings = helperFunctionsSettings;

        _resultCollector = resultCollector;
        _fileModelService = fileModelService;
        _guidGenerator = guidGenerator;
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
        var id = _guidGenerator.GenerateGuid();

        _resultCollector.Add(id, _helperFunctionsSettings.RunIndex, fileName, type);

        await _fileModelService.UploadFileAsync(id.ToString(), content, cancellationToken);
    }

    public void UploadResult(string fileName, string type, string content)
    {
        var byteArray = Encoding.ASCII.GetBytes(content);
        using var stream = new MemoryStream(byteArray);

        UploadResult(fileName, type, stream);
    }

    public void UploadResult(string fileName, string type, Stream content)
    {
        var id = _guidGenerator.GenerateGuid();

        _resultCollector.Add(id, _helperFunctionsSettings.RunIndex, fileName, type);

        _fileModelService.UploadFile(id.ToString(), content);
    }
}
