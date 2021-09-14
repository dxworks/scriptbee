namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class NestedGenericModel : GenericModel<int>
    {
        public GenericModel<string> generic;
        public GenericModel<GenericModel<int>, DummyModel> generic2 { get; set; }

        public GenericModel<DummyModelWithMethods, GenericModel<GenericModel<DummyModel>>> GetGenericModel(
            GenericModel<int, string> gen)
        {
            return null;
        }
    }
}