using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.Sdk;
using Xunit.v3;

namespace ScriptBee.Tests.Common;

public class FilePathAttribute : DataAttribute
{
    private readonly string _filePath;
    private readonly string? _parameter;

    public FilePathAttribute(
        string filePath,
        string? parameter = null,
        [CallerFilePath] string testFilePath = ""
    )
    {
        _filePath = GetFilePath(filePath, testFilePath);
        _parameter = parameter;
    }

    public static string GetFilePath(string filePath, [CallerFilePath] string testFilePath = "")
    {
        var testDataFolder = Path.GetRelativePath(
                Directory.GetCurrentDirectory(),
                Directory.GetParent(testFilePath)!.FullName
            )
            .Replace("..\\", "");
        return Path.Combine(Directory.GetCurrentDirectory(), testDataFolder, filePath);
    }

    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
        MethodInfo testMethod,
        DisposalTracker disposalTracker
    )
    {
        if (_parameter is null)
        {
            return ValueTask.FromResult<IReadOnlyCollection<ITheoryDataRow>>(
                [new TheoryDataRow(_filePath)]
            );
        }

        return ValueTask.FromResult<IReadOnlyCollection<ITheoryDataRow>>(
            [new TheoryDataRow(_filePath, _parameter)]
        );
    }

    public override bool SupportsDiscoveryEnumeration()
    {
        return true;
    }
}
