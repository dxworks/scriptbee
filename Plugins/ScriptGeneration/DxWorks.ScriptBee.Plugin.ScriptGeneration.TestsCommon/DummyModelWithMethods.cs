namespace DxWorks.ScriptBee.Plugin.ScriptGeneration.TestsCommon;

public class DummyModelWithMethods
{
    public int DummyNumber { get; set; }
    public string DummyString { get; set; }
    public bool IsDummy { get; set; }

    public string SayHello()
    {
        return "Hello!";
    }

    public int GetDouble()
    {
        return 2 * DummyNumber;
    }

    public void PrintNames(string name1, string name2, int number)
    {
        Console.WriteLine($"Hello {name1}, {name2}, {number}");
    }
}
