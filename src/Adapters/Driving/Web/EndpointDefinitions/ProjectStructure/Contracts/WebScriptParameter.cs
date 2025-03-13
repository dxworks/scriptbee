using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebScriptParameter(string Name, string Type, object? Value)
{
    public static WebScriptParameter Map(ScriptParameter parameter)
    {
        return new WebScriptParameter(parameter.Name, parameter.Type, parameter.Value);
    }
}
