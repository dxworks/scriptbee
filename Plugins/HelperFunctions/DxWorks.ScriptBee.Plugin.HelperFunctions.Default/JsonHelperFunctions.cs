using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;
using Newtonsoft.Json;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Default;

public class JsonHelperFunctions(IHelperFunctionsResultService helperFunctionsResultService)
    : IHelperFunctions
{
    public async Task ExportJsonAsync<T>(
        string fileName,
        T obj,
        JsonSerializerSettings? settings = null,
        CancellationToken cancellationToken = default
    )
    {
        var jsonSerializer = JsonSerializer.Create(settings);
        await using var stream = new MemoryStream();

        await using var writer = new StreamWriter(stream);
        await using var jsonWriter = new JsonTextWriter(writer);
        jsonSerializer.Serialize(jsonWriter, obj);

        await jsonWriter.FlushAsync(cancellationToken);
        stream.Position = 0;
        await helperFunctionsResultService.UploadResultAsync(
            fileName,
            RunResultDefaultTypes.FileType,
            stream,
            cancellationToken
        );
    }

    public void ExportJson(string fileName, object obj)
    {
        var jsonSerializer = JsonSerializer.Create();
        using var stream = new MemoryStream();

        using var writer = new StreamWriter(stream);
        using var jsonWriter = new JsonTextWriter(writer);
        jsonSerializer.Serialize(jsonWriter, obj);

        jsonWriter.Flush();
        stream.Position = 0;
        helperFunctionsResultService.UploadResult(fileName, RunResultDefaultTypes.FileType, stream);
    }

    public string ConvertJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}
