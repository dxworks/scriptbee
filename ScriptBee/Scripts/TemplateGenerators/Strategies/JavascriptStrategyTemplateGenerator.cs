using ScriptBee.Utils;

namespace ScriptBee.Scripts.TemplateGenerators.Strategies
{
    public class JavascriptStrategyTemplateGenerator : IStrategyTemplateGenerator
    {
        public string GenerateClassName(string className)
        {
            return $"class {className}";
        }

        public string GenerateClassStart()
        {
            return "{";
        }

        public string GenerateClassEnd()
        {
            return "}";
        }

        public string GenerateField(string fieldModifier, string fieldType, string fieldName)
        {
            if (fieldType is
                "byte" or "System.Byte" or
                "sbyte" or "System.SByte" or
                "decimal" or "System.Decimal"or
                "double" or "System.Double"or
                "float" or "System.Single"or
                "int" or "System.Int32"or
                "uint" or "System.UInt32"or
                "long" or "System.Int64"or
                "ulong" or "System.UInt64"or
                "short" or "System.Int16"or
                "ushort" or "System.UInt16")
            {
                return $"    {fieldName} = 0;";
            }

            if (fieldType is
                "char" or "System.Char"or
                "string" or "System.String")
            {
                return $"    {fieldName} = \'\';";
            }

            if (fieldType is "bool" or "System.Boolean")
            {
                return $"    {fieldName} = true;";
            }

            return $"    {fieldName} = new {fieldType}();";
        }

        public string GenerateStartComment()
        {
            return ValidScriptDelimiters.JavascriptStartComment;
        }

        public string GenerateEndComment()
        {
            return ValidScriptDelimiters.JavascriptEndComment;
        }

        public string GenerateModelDeclaration(string modelType)
        {
            return $"let model = new {modelType}();";
        }

        public string GenerateSampleCode()
        {
            return "print(model);";
        }
    }
}