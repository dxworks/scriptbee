using System.Globalization;
using CsvHelper;
using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Csv;

public class CsvHelperFunctions : IHelperFunctions
{
    private readonly IHelperFunctionsResultService _helperFunctionsResultService;

    public CsvHelperFunctions(IHelperFunctionsResultService helperFunctionsResultService)
    {
        _helperFunctionsResultService = helperFunctionsResultService;
    }

    public async Task ExportCsvAsync<T>(string fileName, IEnumerable<T> records,
        CancellationToken cancellationToken = default)
    {
        await using var stream = new MemoryStream();

        await using var writer = new StreamWriter(stream);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(records, cancellationToken);

        await _helperFunctionsResultService.UploadResultAsync(fileName, RunResultDefaultTypes.FileType, stream,
            cancellationToken);
    }

    public void ExportCsv(string fileName, List<object> records)
    {
        using var stream = new MemoryStream();

        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(records);

        _helperFunctionsResultService.UploadResult(fileName, RunResultDefaultTypes.FileType, stream);
    }
}
