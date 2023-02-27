namespace DxWorks.ScriptBee.Plugin.ScriptRunner.TestsCommon;

public class NestedGenericModel : GenericModel<int>
{
    public GenericModel<string> generic = null!;
    public GenericModel<GenericModel<int>, DummyModel> generic2 { get; set; } = null!;

    public GenericModel<DummyModelWithMethods, GenericModel<GenericModel<DummyModel>>> GetGenericModel(
        GenericModel<int, string> gen)
    {
        return null!;
    }
}
