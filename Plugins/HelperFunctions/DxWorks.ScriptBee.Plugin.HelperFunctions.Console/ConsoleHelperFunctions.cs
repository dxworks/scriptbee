using System.Globalization;
using System.Text;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Console;

public class ConsoleHelperFunctions : IHelperFunctions
{
    private readonly IHelperFunctionsResultService _helperFunctionsResultService;

    private readonly StringBuilder _consoleStringBuilder = new();

    public ConsoleHelperFunctions(IHelperFunctionsResultService helperFunctionsResultService)
    {
        _helperFunctionsResultService = helperFunctionsResultService;
    }

    public async Task OnUnloadAsync(CancellationToken cancellationToken = default)
    {
        var consoleOutput = _consoleStringBuilder.ToString();

        await _helperFunctionsResultService.UploadResultAsync("ConsoleOutput", RunResultDefaultTypes.ConsoleType,
            consoleOutput, cancellationToken);
    }
    // todo add more overloads for bool, long, float, etc.

    public void ConsoleWrite(object text)
    {
        _consoleStringBuilder.Append(text);
    }

    public void ConsoleWrite(string message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(int message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(double message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWriteLine(string message)
    {
        _consoleStringBuilder.AppendLine(message);
    }

    public void ConsoleWriteLine(object message)
    {
        _consoleStringBuilder.AppendLine(message.ToString());
    }

    public void ConsoleWriteLine(int message)
    {
        _consoleStringBuilder.AppendLine(message.ToString());
    }

    public void ConsoleWriteLine(double message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }
}
