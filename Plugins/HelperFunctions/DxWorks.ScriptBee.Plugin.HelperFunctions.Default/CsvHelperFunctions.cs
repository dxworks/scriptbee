using System.Dynamic;
using System.Reflection;
using CsvHelper;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Default;

public class CsvHelperFunctions(IHelperFunctionsResultService helperFunctionsResultService)
    : IHelperFunctions
{
    public async Task ExportCsvAsync<T>(
        string fileName,
        IEnumerable<T> records,
        CancellationToken cancellationToken = default
    )
    {
        await using var stream = new MemoryStream();

        await using (var csv = CsvWriter.Create(stream, leaveOpen: true))
        {
            await csv.WriteRecordsAsync(
                records.Select(r => (object)ToExpando(r)),
                cancellationToken
            );
            await csv.FlushAsync(cancellationToken);
        }

        stream.Position = 0;

        await helperFunctionsResultService.UploadResultAsync(
            fileName,
            RunResultDefaultTypes.FileType,
            stream,
            cancellationToken
        );
    }

    public void ExportCsv(string fileName, List<object> records)
    {
        using var stream = new MemoryStream();

        using (var csv = CsvWriter.Create(stream, leaveOpen: true))
        {
            csv.WriteRecords(records.Select(ToExpando).Cast<object>());
            csv.Flush();
        }

        stream.Position = 0;

        helperFunctionsResultService.UploadResult(fileName, RunResultDefaultTypes.FileType, stream);
    }

    private static ExpandoObject ToExpando(object? obj)
    {
        var expando = new ExpandoObject();
        IDictionary<string, object?> dict = expando;

        if (obj is IDictionary<string, object?> existing)
        {
            foreach (var kvp in existing)
                dict[kvp.Key] = kvp.Value;

            return expando;
        }

        foreach (
            var property in obj?.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                ?? []
        )
        {
            dict[property.Name] = property.GetValue(obj);
        }

        return expando;
    }
}
