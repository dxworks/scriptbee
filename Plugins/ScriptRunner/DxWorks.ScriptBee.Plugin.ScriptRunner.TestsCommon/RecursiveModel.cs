namespace DxWorks.ScriptBee.Plugin.ScriptRunner.TestsCommon;

public class RecursiveModel
{
    public long longField;

    public DummyModel dummyField1 = null!;

    public DummyModel dummyField2 { get; set; } = null!;

    public RecursiveModel recursiveModel = null!;
}

public class RecursiveModel2
{
    public DummyModel dummyField1 = null!;

    public DummyModel dummyField2 = null!;

    public char value;
}
