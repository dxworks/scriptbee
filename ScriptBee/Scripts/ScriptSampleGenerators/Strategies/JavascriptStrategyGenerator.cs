﻿using ScriptBee.Utils;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class JavascriptStrategyGenerator : IStrategyGenerator
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
                "byte" or "System.Byte" or "Byte" or
                "sbyte" or "System.SByte" or "SByte" or
                "decimal" or "System.Decimal"or "Decimal" or
                "double" or "System.Double"or "Double" or
                "float" or "System.Single"or "Single" or
                "int" or "System.Int32"or "Int32" or
                "uint" or "System.UInt32"or "UInt32" or
                "long" or "System.Int64"or "Int64" or
                "ulong" or "System.UInt64"or "UInt64" or
                "short" or "System.Int16"or "Int16" or
                "ushort" or "System.UInt16"or "UInt16")
            {
                return $"    {fieldName} = 0;";
            }

            if (fieldType is
                "char" or "System.Char" or "Char" or
                "string" or "System.String" or "String")
            {
                return $"    {fieldName} = \'\';";
            }

            if (fieldType is "bool" or "System.Boolean" or "Boolean")
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

        public string GenerateEmptyClass()
        {
            return "";
        }
    }
}