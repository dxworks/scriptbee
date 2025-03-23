using System.Text;
using DxWorks.ScriptBee.Plugin.Api.Services;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Ports.Files;

namespace ScriptBee.Service.Analysis;

public class HelperFunctionsResultService(
    HelperFunctionsSettings helperFunctionsSettings,
    IResultCollector resultCollector,
    IFileModelService fileModelService,
    IGuidProvider guidProvider
) : IHelperFunctionsResultService
{
    public async Task UploadResultAsync(
        string fileName,
        string type,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        var byteArray = Encoding.ASCII.GetBytes(content);
        await using var stream = new MemoryStream(byteArray);

        await UploadResultAsync(fileName, type, stream, cancellationToken);
    }

    public async Task UploadResultAsync(
        string fileName,
        string type,
        Stream content,
        CancellationToken cancellationToken = default
    )
    {
        var resultId = new ResultId(guidProvider.NewGuid());

        resultCollector.Add(resultId, helperFunctionsSettings, fileName, type);

        await fileModelService.UploadFileAsync(resultId.ToFileId(), content, cancellationToken);
    }

    public void UploadResult(string fileName, string type, string content)
    {
        var byteArray = Encoding.ASCII.GetBytes(content);
        using var stream = new MemoryStream(byteArray);

        UploadResult(fileName, type, stream);
    }

    public void UploadResult(string fileName, string type, Stream content)
    {
        var resultId = new ResultId(guidProvider.NewGuid());

        resultCollector.Add(resultId, helperFunctionsSettings, fileName, type);

        fileModelService.UploadFile(resultId.ToFileId(), content);
    }
}
