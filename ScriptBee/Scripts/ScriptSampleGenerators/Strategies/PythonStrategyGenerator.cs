﻿using System;
using System.Collections.Generic;
using System.Text;
using ScriptBee.Utils;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class PythonStrategyGenerator : IStrategyGenerator
    {
        private readonly IFileContentProvider _fileContentProvider;

        public PythonStrategyGenerator(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public string GenerateClassName(string className)
        {
            return $"class {className}:";
        }
        
        public string GenerateClassName(string className, string superClassName)
        {
            return $"class {className}({superClassName}):";
        }

        public string GenerateClassStart()
        {
            return "";
        }

        public string GenerateClassEnd()
        {
            return "";
        }

        public string GenerateField(string fieldModifier, Type fieldType, string fieldName)
        {
            var fieldTypeName = GetTypeName(fieldType);
            if (fieldTypeName is
                "decimal" or "System.Decimal"or "Decimal"or
                "double" or "System.Double"or "Double"or
                "float" or "System.Single" or "Single")
            {
                return $"    {fieldName}: float";
            }

            if (fieldTypeName is
                "byte" or "System.Byte" or "Byte" or
                "sbyte" or "System.SByte" or "SByte" or
                "int" or "System.Int32"or "Int32" or
                "uint" or "System.UInt32"or "UInt32" or
                "short" or "System.Int16"or "Int16" or
                "ushort" or "System.UInt16" or "UInt16")
            {
                return $"    {fieldName}: int";
            }

            if (fieldTypeName is
                "long" or "System.Int64"or "Int64" or
                "ulong" or "System.UInt64" or "UInt64")
            {
                return $"    {fieldName}: long";
            }

            if (fieldTypeName is
                "char" or "System.Char"or "Char" or
                "string" or "System.String" or "String")
            {
                return $"    {fieldName}: str";
            }

            if (fieldTypeName is "bool" or "System.Boolean" or "Boolean")
            {
                return $"    {fieldName}: bool";
            }

            return $"    {fieldName}: {fieldTypeName}";
        }

        public string GenerateProperty(string propertyModifier, Type propertyType, string propertyName)
        {
            return GenerateField(propertyModifier, propertyType, propertyName);
        }

        public string GenerateMethod(string methodModifier, Type methodType, string methodName, List<Tuple<string, string>> methodParams)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"    def {methodName}(");
            
            for (var i = 0; i < methodParams.Count; i++)
            {
                var tuple = methodParams[i];
                stringBuilder.Append($"{tuple.Item2}");
                if (i != methodParams.Count - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.AppendLine("):");
            stringBuilder.AppendLine("        pass");

            return stringBuilder.ToString();
        }

        public string GenerateModelDeclaration(string modelType)
        {
            return $"project: {modelType}";
        }

        public string GenerateSampleCode()
        {
            return _fileContentProvider.GetFileContent(
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

        private string GetTypeName(Type type)
        {
            return type.Name;
        }
    }
}