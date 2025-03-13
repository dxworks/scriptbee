namespace DxWorks.ScriptBee.Plugin.Api.Model;

public class ScriptParameter
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public object? Value { get; set; }

    public const string TypeString = "string";
    public const string TypeInteger = "integer";
    public const string TypeBoolean = "boolean";
    public const string TypeFloat = "float";
}
