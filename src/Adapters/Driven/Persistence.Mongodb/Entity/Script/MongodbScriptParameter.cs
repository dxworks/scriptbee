using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.Persistence.Mongodb.Entity.Script;

public class MongodbScriptParameter
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public object? Value { get; set; }

    public static MongodbScriptParameter From(ScriptParameter parameter)
    {
        return new MongodbScriptParameter
        {
            Name = parameter.Name,
            Type = parameter.Type,
            Value = parameter.Value,
        };
    }
}
