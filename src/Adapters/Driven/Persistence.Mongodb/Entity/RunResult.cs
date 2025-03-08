namespace ScriptBee.Persistence.Mongodb.Entity;

public class RunResult
{
    public Guid Id { get; set; }

    public int RunIndex { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
}
