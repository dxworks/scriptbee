namespace HelperFunctions;

public interface IConsoleOutputWriter
{
    void ConsoleWrite(string message);

    void ConsoleWriteLine(string message);
}