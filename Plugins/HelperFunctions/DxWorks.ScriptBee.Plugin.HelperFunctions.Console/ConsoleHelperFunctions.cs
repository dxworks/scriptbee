using System.Text;
using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Console;

public class ConsoleHelperFunctions : IHelperFunctions
{
    private readonly HelperFunctionsSettings _helperFunctionsSettings;
    private readonly IFileModelService _fileModelService;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IResultCollector _resultCollector;

    private readonly StringBuilder _consoleStringBuilder = new();

    public ConsoleHelperFunctions(HelperFunctionsSettings helperFunctionsSettings, IFileModelService fileModelService,
        IFileNameGenerator fileNameGenerator, IResultCollector resultCollector)
    {
        _helperFunctionsSettings = helperFunctionsSettings;
        _fileModelService = fileModelService;
        _fileNameGenerator = fileNameGenerator;
        _resultCollector = resultCollector;
    }

    public void ConsoleWrite(string message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWriteLine(string message)
    {
        _consoleStringBuilder.AppendLine(message);
    }

    public async Task OnUnloadAsync(CancellationToken cancellationToken = default)
    {
        var consoleOutput = _consoleStringBuilder.ToString();
        if (string.IsNullOrEmpty(consoleOutput))
        {
            return;
        }

        var consoleOutputName =
            _fileNameGenerator.GenerateOutputFileName(_helperFunctionsSettings.ProjectId,
                _helperFunctionsSettings.RunId, RunResult.ConsoleType, "ConsoleOutput");
        _resultCollector.Add(new RunResult(RunResult.ConsoleType, consoleOutputName));

        var byteArray = Encoding.ASCII.GetBytes(consoleOutput);
        await using var stream = new MemoryStream(byteArray);

        await _fileModelService.UploadFileAsync(consoleOutputName, stream, cancellationToken);
    }
}
