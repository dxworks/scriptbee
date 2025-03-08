namespace ScriptBee.Persistence.Mongodb.Entity;

public class Run
{
    public int Index { get; set; }

    public Dictionary<string, List<FileData>> LoadedFiles { get; set; } = new();
    public string Linker { get; set; } = "";
    public string ScriptPath { get; set; } = "";
    public Guid ScriptId { get; set; }
    public List<RunResult> Results { get; set; } = new();
}
