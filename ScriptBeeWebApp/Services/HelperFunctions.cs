using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using HelperFunctions;
using Newtonsoft.Json;

namespace ScriptBeeWebApp.Services;

public class HelperFunctions : IHelperFunctionsWithResults
{
    private readonly string _projectId;
    private readonly string _runId;
    private readonly List<RunResult> _results = new();
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly StringBuilder _consoleStringBuilder = new();
    private const string ConsoleOutput = "ConsoleOutput";

    public HelperFunctions(string projectId, string runId, IFileModelService fileModelService,
        IFileNameGenerator fileNameGenerator)
    {
        _projectId = projectId;
        _runId = runId;
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
    }

    public async void FileWrite(string fileName, string fileContent)
    {
        string outputFileName =
            _fileNameGenerator.GenerateOutputFileName(_projectId, _runId, RunResult.FileType, fileName);

        _results.Add(new RunResult(RunResult.FileType, outputFileName));

        var byteArray = Encoding.ASCII.GetBytes(fileContent);
        await using var stream = new MemoryStream(byteArray);

        await _fileModelService.UploadFile(outputFileName, stream);
    }

    public async void FileWriteStream(string fileName, Stream stream)
    {
        string outputFileName =
            _fileNameGenerator.GenerateOutputFileName(_projectId, _runId, RunResult.FileType, fileName);

        _results.Add(new RunResult(RunResult.FileType, outputFileName));

        await _fileModelService.UploadFile(outputFileName, stream);
    }

    public async void ExportJson<T>(string fileName, T obj)
        // public async void ExportJson<T>(string fileName, T obj, JsonSerializerSettings? settings = default)
    {
        var outputJsonName =
            _fileNameGenerator.GenerateOutputFileName(_projectId, _runId, RunResult.FileType, fileName);

        _results.Add(new RunResult(RunResult.FileType, outputJsonName));

        var jsonSerializer = JsonSerializer.Create();
        await using var stream = new MemoryStream();

        await using var writer = new StreamWriter(stream);
        using var jsonWriter = new JsonTextWriter(writer);
        jsonSerializer.Serialize(jsonWriter, obj);

        // stream.Position = 0;
        await _fileModelService.UploadFile(outputJsonName, stream);
    }


    public async void ExportCsv<T>(string fileName, List<T> records)
    {
        var outputCsvName = _fileNameGenerator.GenerateOutputFileName(_projectId, _runId, RunResult.FileType, fileName);

        _results.Add(new RunResult(RunResult.FileType, outputCsvName));

        await using var stream = new MemoryStream();

        await using var writer = new StreamWriter(stream);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(records);

        await _fileModelService.UploadFile(outputCsvName, stream);
    }

    public void ConsoleWrite(string message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWriteLine(string message)
    {
        _consoleStringBuilder.AppendLine(message);
    }

    public async Task<List<RunResult>> GetResults()
    {
        var consoleOutput = _consoleStringBuilder.ToString();
        if (string.IsNullOrEmpty(consoleOutput))
        {
            return _results;
        }

        string consoleOutputName =
            _fileNameGenerator.GenerateOutputFileName(_projectId, _runId, RunResult.ConsoleType, ConsoleOutput);
        _results.Add(new RunResult(RunResult.ConsoleType, consoleOutputName));

        var byteArray = Encoding.ASCII.GetBytes(consoleOutput);
        await using var stream = new MemoryStream(byteArray);

        await _fileModelService.UploadFile(consoleOutputName, stream);

        return _results;
    }
}