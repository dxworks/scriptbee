namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Default.Charts;

public class ChartData
{
    public Dictionary<string, object> options { get; set; } = new();
    public List<Dictionary<string, object>> series { get; set; } = [];
    public string type { get; set; } = "";
}
