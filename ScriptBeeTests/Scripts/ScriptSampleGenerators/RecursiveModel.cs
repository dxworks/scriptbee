namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class RecursiveModel
    {
        public long longField;

        public DummyModel dummyField1;

        public DummyModel dummyField2 { get; set; }

        public RecursiveModel recursiveModel;
    }

    public class RecursiveModel2
    {
        public DummyModel dummyField1;

        public DummyModel dummyField2;

        public char value;
    }
}