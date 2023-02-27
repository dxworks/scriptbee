namespace DxWorks.ScriptBee.Plugin.ScriptRunner.TestsCommon;

public class DummyModelInheritor : DummyModel
{
    public bool IsBetter { get; set; }

    public string SayBye()
    {
        return "Bye!";
    }

}
