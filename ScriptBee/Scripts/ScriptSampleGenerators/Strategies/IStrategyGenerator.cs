namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public interface IStrategyGenerator
    {
        public string GenerateClassName(string className);

        public string GenerateClassStart();

        public string GenerateClassEnd();

        public string GenerateField(string fieldModifier, string fieldType, string fieldName);
        
        public string GenerateProperty(string propertyModifier, string propertyType, string propertyName);

        public string GenerateModelDeclaration(string modelType);

        public string GenerateSampleCode();

        public string GenerateEmptyClass();

        public string GenerateImports();

        public string GetStartComment();

        public string GetEndComment();
    }
}