using ScriptBee.Utils;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class PythonStrategyGenerator : IStrategyGenerator
    {
        private readonly ISampleCodeProvider _sampleCodeProvider;

        public PythonStrategyGenerator(ISampleCodeProvider sampleCodeProvider)
        {
            _sampleCodeProvider = sampleCodeProvider;
        }

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

        public string GenerateProperty(string propertyModifier, string propertyType, string propertyName)
        {
            return GenerateField(propertyModifier, propertyType, propertyName);
        }

        public string GenerateModelDeclaration(string modelType)
        {
            return $"model: {modelType}";
        }

        public string GenerateSampleCode()
        {
            return _sampleCodeProvider.GetSampleCode(
                "Scripts/ScriptSampleGenerators/Strategies/SampleCodes/PythonSampleCode.txt");
        }

        public string GenerateEmptyClass()
        {
            return "    pass";
        }

        public string GenerateImports()
        {
            return "";
        }

        public string GetStartComment()
        {
            return ValidScriptDelimiters.PythonStartComment;
        }

        public string GetEndComment()
        {
            return ValidScriptDelimiters.PythonEndComment;
        }
    }
}