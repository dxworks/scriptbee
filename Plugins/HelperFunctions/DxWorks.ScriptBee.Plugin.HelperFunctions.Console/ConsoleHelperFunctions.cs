using System.Text;
using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Console;

public class ConsoleHelperFunctions : IHelperFunctions
{
    private readonly IHelperFunctionsResultService _helperFunctionsResultService;

    private readonly StringBuilder _consoleStringBuilder = new();

    public ConsoleHelperFunctions(IHelperFunctionsResultService helperFunctionsResultService)
    {
        _helperFunctionsResultService = helperFunctionsResultService;
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

        await _helperFunctionsResultService.UploadResultAsync("ConsoleOutput", RunResultDefaultTypes.ConsoleType,
            consoleOutput, cancellationToken);
    }
}
