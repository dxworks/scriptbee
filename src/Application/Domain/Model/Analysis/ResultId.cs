using ScriptBee.Domain.Model.File;

namespace ScriptBee.Domain.Model.Analysis;

public readonly record struct ResultId(Guid Value)
{
    public ResultId(string value)
        : this(Guid.Parse(value)) { }

    public FileId ToFileId() => new(Value);

    public override string ToString() => Value.ToString();
}
