using System.Text;
using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.File;

public class FileHelperFunctions
{
    private readonly HelperFunctionsSettings _helperFunctionsSettings;
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IResultCollector _resultCollector;

    public FileHelperFunctions(HelperFunctionsSettings helperFunctionsSettings, IFileModelService fileModelService,
        IFileNameGenerator fileNameGenerator, IResultCollector resultCollector)
    {
        _helperFunctionsSettings = helperFunctionsSettings;
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
        _resultCollector = resultCollector;
    }

    public void FileWrite(string fileName, string fileContent)
    {
        var outputFileName =
            _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
                _helperFunctionsSettings.RunId, RunResult.FileType, fileName);

        _resultCollector.Add(new RunResult(RunResult.FileType, outputFileName));

        var byteArray = Encoding.ASCII.GetBytes(fileContent);
        using var stream = new MemoryStream(byteArray);

        _fileModelService.UploadFile(outputFileName, stream);
    }

    public async Task FileWriteAsync(string fileName, string fileContent)
    {
        var outputFileName =
            _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
                _helperFunctionsSettings.RunId, RunResult.FileType, fileName);

        _resultCollector.Add(new RunResult(RunResult.FileType, outputFileName));

        var byteArray = Encoding.ASCII.GetBytes(fileContent);
        await using var stream = new MemoryStream(byteArray);

        await _fileModelService.UploadFileAsync(outputFileName, stream);
    }

    public async Task FileWriteStreamAsync(string fileName, Stream stream)
    {
        var outputFileName =
            _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
                _helperFunctionsSettings.RunId, RunResult.FileType, fileName);

        _resultCollector.Add(new RunResult(RunResult.FileType, outputFileName));

        await _fileModelService.UploadFileAsync(outputFileName, stream);
    }

    public void FileWriteStream(string fileName, Stream stream)
    {
        var outputFileName =
            _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
                _helperFunctionsSettings.RunId, RunResult.FileType, fileName);

        _resultCollector.Add(new RunResult(RunResult.FileType, outputFileName));

        _fileModelService.UploadFile(outputFileName, stream);
    }
}
