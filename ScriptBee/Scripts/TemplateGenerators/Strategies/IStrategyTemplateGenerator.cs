namespace ScriptBee.Scripts.TemplateGenerators.Strategies
{
    public interface IStrategyTemplateGenerator
    {
        public string GenerateClassName(string className);

        public string GenerateClassStart();

        public string GenerateClassEnd();

        public string GenerateField(string fieldModifier, string fieldType, string fieldName);

        public string GenerateStartComment();

        public string GenerateEndComment();

        public string GenerateModelDeclaration(string modelType);

        public string GenerateSampleCode();

        public string GenerateEmptyClass();
    }
}