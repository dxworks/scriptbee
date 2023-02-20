namespace ScriptBee.Models;

public class ScriptParameterModel
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? Value { get; set; }

    public const string TypeString = "string";
    public const string TypeInteger = "integer";
    public const string TypeBoolean = "boolean";
    public const string TypeFloat = "float";
}
