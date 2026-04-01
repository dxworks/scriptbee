using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Validation;

public static class AllowedScriptParameterTypes
{
    public static readonly List<string> AllowedTypes =
    [
        ScriptParameter.TypeString,
        ScriptParameter.TypeInteger,
        ScriptParameter.TypeFloat,
        ScriptParameter.TypeBoolean,
    ];
}
