using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;
using Newtonsoft.Json;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Json;

public class JsonHelperFunctions : IHelperFunctions
{
    private readonly HelperFunctionsSettings _helperFunctionsSettings;
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IResultCollector _resultCollector;

    public JsonHelperFunctions(HelperFunctionsSettings helperFunctionsSettings, IFileModelService fileModelService,
        IFileNameGenerator fileNameGenerator, IResultCollector resultCollector)
    {
        _helperFunctionsSettings = helperFunctionsSettings;
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
        _resultCollector = resultCollector;
    }

    public async Task ExportJsonAsync<T>(string fileName, T obj, JsonSerializerSettings? settings = default)
    {
        var outputJsonName =
            _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
                _helperFunctionsSettings.RunId, RunResult.FileType, fileName);

        _resultCollector.Add(new RunResult(RunResult.FileType, outputJsonName));

        var jsonSerializer = JsonSerializer.Create(settings);
        await using var stream = new MemoryStream();

        await using var writer = new StreamWriter(stream);
        using var jsonWriter = new JsonTextWriter(writer);
        jsonSerializer.Serialize(jsonWriter, obj);

        stream.Position = 0;
        await _fileModelService.UploadFileAsync(outputJsonName, stream);
    }

    public void ExportJson(string fileName, object obj)
    {
        var outputJsonName =
            _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
                _helperFunctionsSettings.RunId, RunResult.FileType, fileName);

        _resultCollector.Add(new RunResult(RunResult.FileType, outputJsonName));

        var jsonSerializer = JsonSerializer.Create();
        using var stream = new MemoryStream();

        using var writer = new StreamWriter(stream);
        using var jsonWriter = new JsonTextWriter(writer);
        jsonSerializer.Serialize(jsonWriter, obj);

        stream.Position = 0;
        _fileModelService.UploadFile(outputJsonName, stream);
    }
}
