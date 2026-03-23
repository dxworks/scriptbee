namespace ScriptBee.Domain.Model.File;

public readonly record struct FileId(Guid Value)
{
    public FileId(string value)
        : this(Guid.Parse(value)) { }

    public override string ToString() => Value.ToString();
}
