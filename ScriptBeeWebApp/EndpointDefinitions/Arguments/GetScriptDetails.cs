using System.Reflection;

namespace ScriptBeeWebApp.EndpointDefinitions.Arguments;

public class GetScriptDetails
{
    public string ProjectId { get; init; } = null!;
    public string FilePath { get; init; } = null!;

    // todo: replace with AsParameters when upgrading to .NET 7
    public static ValueTask<GetScriptDetails?> BindAsync(HttpContext context, ParameterInfo parameterInfo)
    {
        var projectId = context.Request.Query["projectId"];
        var filePath = context.Request.Query["filePath"];

        var getScriptDetails = new GetScriptDetails
        {
            ProjectId = projectId,
            FilePath = filePath
        };

        return ValueTask.FromResult<GetScriptDetails?>(getScriptDetails);
    }

    public static bool TryParse(string? value, out GetScriptDetails? result)
    {
        result = null;
        if (value is null)
        {
            return false;
        }

        var parts = value.Split(',');
        if (parts.Length != 2)
        {
            return false;
        }

        result = new GetScriptDetails
        {
            ProjectId = parts[0],
            FilePath = parts[1]
        };

        return true;
    }
}
