using DxWorks.ScriptBee.Plugin.Api.Services;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.File;

public class FileHelperFunctions
{
    private readonly IHelperFunctionsResultService _helperFunctionsResultService;

    public FileHelperFunctions(IHelperFunctionsResultService helperFunctionsResultService)
    {
        _helperFunctionsResultService = helperFunctionsResultService;
    }

    public void FileWrite(string fileName, string fileContent)
    {
        _helperFunctionsResultService.UploadResult(fileName, RunResultDefaultTypes.FileType, fileContent);
    }

    public async Task FileWriteAsync(string fileName, string fileContent, CancellationToken cancellationToken = default)
    {
        await _helperFunctionsResultService.UploadResultAsync(fileName, RunResultDefaultTypes.FileType, fileContent,
            cancellationToken);
    }

    public async Task FileWriteStreamAsync(string fileName, Stream stream,
        CancellationToken cancellationToken = default)
    {
        await _helperFunctionsResultService.UploadResultAsync(fileName, RunResultDefaultTypes.FileType, stream,
            cancellationToken);
    }

    public void FileWriteStream(string fileName, Stream stream)
    {
        _helperFunctionsResultService.UploadResult(fileName, RunResultDefaultTypes.FileType, stream);
    }
}
