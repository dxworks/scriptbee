using System.Globalization;
using System.Text;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Default;

public class ConsoleHelperFunctions : IHelperFunctions
{
    private readonly StringBuilder _consoleStringBuilder = new();
    private readonly IHelperFunctionsResultService _helperFunctionsResultService;

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

    public void ConsoleWrite(object message)
    {
        _consoleStringBuilder.Append(message);
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

    public void ConsoleWrite(float message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(bool message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(long message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(decimal message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(DateTime message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(char message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(short message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(uint message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(ulong message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWrite(ushort message)
    {
        _consoleStringBuilder.Append(message);
    }

    public void ConsoleWriteLine(object message)
    {
        _consoleStringBuilder.AppendLine(message.ToString());
    }

    public void ConsoleWriteLine(string message)
    {
        _consoleStringBuilder.AppendLine(message);
    }

    public void ConsoleWriteLine(int message)
    {
        _consoleStringBuilder.AppendLine(message.ToString());
    }

    public void ConsoleWriteLine(double message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }

    public void ConsoleWriteLine(float message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }

    public void ConsoleWriteLine(bool message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }

    public void ConsoleWriteLine(long message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }

    public void ConsoleWriteLine(decimal message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }
    
    public void ConsoleWriteLine(DateTime message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }

    public void ConsoleWriteLine(char message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }
    
    public void ConsoleWriteLine(short message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }

    public void ConsoleWriteLine(uint message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }

    public void ConsoleWriteLine(ulong message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }

    public void ConsoleWriteLine(ushort message)
    {
        _consoleStringBuilder.AppendLine(message.ToString(CultureInfo.InvariantCulture));
    }
}
