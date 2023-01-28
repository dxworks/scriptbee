namespace DxWorks.ScriptBee.Plugin.ScriptGeneration.TestsCommon;

public class DummyModelInheritor : DummyModel
{
    public bool IsBetter { get; set; }

    public string SayBye()
    {
        return "Bye!";
    }

}
