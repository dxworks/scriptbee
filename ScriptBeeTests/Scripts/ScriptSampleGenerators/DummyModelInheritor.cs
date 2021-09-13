namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class DummyModelInheritor : DummyModel
    {
        public bool IsBetter { get; set; }

        public string SayBye()
        {
            return "Bye!";
        }

    }
}