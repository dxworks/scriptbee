namespace ScriptBeePlugin
{
    public interface IModelLoader
    {
        public ScriptBeeModel LoadModel(string fileContent);
    }
}