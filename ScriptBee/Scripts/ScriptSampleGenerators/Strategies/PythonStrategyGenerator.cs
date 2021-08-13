using ScriptBee.Utils;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class PythonStrategyGenerator : IStrategyGenerator
    {
        public string GenerateClassName(string className)
        {
            return $"class {className}:";
        }

        public string GenerateClassStart()
        {
            return "";
        }

        public string GenerateClassEnd()
        {
            return "";
        }

        public string GenerateField(string fieldModifier, string fieldType, string fieldName)
        {
            if (fieldType is
                "decimal" or "System.Decimal"or "Decimal"or
                "double" or "System.Double"or "Double"or
                "float" or "System.Single" or "Single")
            {
                return $"    {fieldName}: float";
            }

            if (fieldType is
                "byte" or "System.Byte" or "Byte" or
                "sbyte" or "System.SByte" or "SByte" or
                "int" or "System.Int32"or "Int32" or
                "uint" or "System.UInt32"or "UInt32" or
                "short" or "System.Int16"or "Int16" or
                "ushort" or "System.UInt16" or "UInt16")
            {
                return $"    {fieldName}: int";
            }

            if (fieldType is
                "long" or "System.Int64"or "Int64" or
                "ulong" or "System.UInt64" or "UInt64")
            {
                return $"    {fieldName}: long";
            }

            if (fieldType is
                "char" or "System.Char"or "Char" or
                "string" or "System.String" or "String")
            {
                return $"    {fieldName}: str";
            }

            if (fieldType is "bool" or "System.Boolean" or "Boolean")
            {
                return $"    {fieldName}: bool";
            }

            return $"    {fieldName}: {fieldType}";
        }

        public string GenerateStartComment()
        {
            return ValidScriptDelimiters.PythonStartComment;
        }

        public string GenerateEndComment()
        {
            return ValidScriptDelimiters.PythonEndComment;
        }

        public string GenerateModelDeclaration(string modelType)
        {
            return $"model: {modelType}";
        }

        public string GenerateSampleCode()
        {
            return "print(model)";
        }

        public string GenerateEmptyClass()
        {
            return "    pass";
        }
    }
}