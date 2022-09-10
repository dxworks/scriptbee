using System.Globalization;
using CsvHelper;
using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Csv;

public class CsvHelperFunctions : IHelperFunctions
{
    private readonly HelperFunctionsSettings _helperFunctionsSettings;
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IResultCollector _resultCollector;

    public CsvHelperFunctions(HelperFunctionsSettings helperFunctionsSettings, IFileModelService fileModelService,
        IFileNameGenerator fileNameGenerator, IResultCollector resultCollector)
    {
        _helperFunctionsSettings = helperFunctionsSettings;
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
        _resultCollector = resultCollector;
    }

    public async Task ExportCsvAsync<T>(string fileName, List<T> records)
    {
        var outputCsvName = _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
            _helperFunctionsSettings.RunId, RunResult.FileType, fileName);

        _resultCollector.Add(new RunResult(RunResult.FileType, outputCsvName));

        await using var stream = new MemoryStream();

        await using var writer = new StreamWriter(stream);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(records);

        await _fileModelService.UploadFileAsync(outputCsvName, stream);
    }

    public void ExportCsv(string fileName, List<object> records)
    {
        var outputCsvName = _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
            _helperFunctionsSettings.RunId, RunResult.FileType, fileName);

        _resultCollector.Add(new RunResult(RunResult.FileType, outputCsvName));

        using var stream = new MemoryStream();

        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(records);

        _fileModelService.UploadFile(outputCsvName, stream);
    }
}
