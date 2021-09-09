using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptBee.Utils;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class CSharpStrategyGenerator : IStrategyGenerator
    {
        private string _modelType;

        private readonly IFileContentProvider _fileContentProvider;

        private const string StartComment = "// Only the code written in the ExecuteScript method will be executed";

        public CSharpStrategyGenerator(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public string GenerateClassName(string className)
        {
            return $"public class {className}";
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
            fieldType = GetPrimitiveType(fieldType);
            return $"    {fieldModifier} {fieldType} {fieldName};";
        }

        public string GenerateProperty(string propertyModifier, string propertyType, string propertyName)
        {
            propertyType = GetPrimitiveType(propertyType);
            return $"    {propertyModifier} {propertyType} {propertyName} {{ get; set; }}";
        }

        public string GenerateMethod(string methodModifier, string methodType, string methodName, List<Tuple<string, string>> methodParams)
        {
            var stringBuilder = new StringBuilder();
            methodType = GetMethodType(methodType);
            stringBuilder.Append($"    {methodModifier} {methodType} {methodName}(");
            
            for (var i = 0; i < methodParams.Count; i++)
            {
                var tuple = methodParams[i];
                var type = GetPrimitiveType(tuple.Item1);
                stringBuilder.Append($"{type} {tuple.Item2}");
                if (i != methodParams.Count - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.AppendLine(")");
            stringBuilder.AppendLine("    {");

            if (methodType != "void")
            {
                stringBuilder.AppendLine("        return default;");
            }
            
            stringBuilder.AppendLine("    }");

            return stringBuilder.ToString();
        }

        public string GenerateModelDeclaration(string modelType)
        {
            _modelType = modelType;
            return "";
        }

        public string GenerateSampleCode()
        {
            return _fileContentProvider.GetFileContent(
                "Scripts/ScriptSampleGenerators/Strategies/SampleCodes/CSharpSampleCode.txt");
        }

        public string GenerateEmptyClass()
        {
            return "";
        }

        public string GenerateImports()
        {
            return _fileContentProvider.GetFileContent(
                "Scripts/ScriptSampleGenerators/Strategies/SampleCodes/CSharpImports.txt");
        }

        public string GetStartComment()
        {
            return StartComment;
        }

        public string GetEndComment()
        {
            return "";
        }

        private string GetPrimitiveType(string type)
        {
            switch (type)
            {
                case "System.Byte":
                case "Byte":
                {
                    return "byte";
                }
                case "System.SByte":
                case "SByte":
                {
                    return "sbyte";
                }
                case "System.Decimal":
                case "Decimal":
                {
                    return "decimal";
                }
                case "System.Double":
                case "Double":
                {
                    return "double";
                }
                case "System.Single":
                case "Single":
                {
                    return "float";
                }
                case "System.Int32":
                case "Int32":
                {
                    return "int";
                }
                case "System.UInt32":
                case "UInt32":
                {
                    return "uint";
                }
                case "System.Int64":
                case "Int64":
                {
                    return "long";
                }
                case "System.UInt64":
                case "UInt64":
                {
                    return "ulong";
                }
                case "System.Int16":
                case "Int16":
                {
                    return "short";
                }
                case "System.UInt16":
                case "UInt16":
                {
                    return "ushort";
                }
                case "System.String":
                case "String":
                {
                    return "string";
                }
                case "System.Char":
                case "Char":
                {
                    return "char";
                }
                case "System.Boolean":
                case "Boolean":
                {
                    return "bool";
                }
            }

            return type;
        }
        
        private string GetMethodType(string type)
        {
            switch (type)
            {
                case "Void":
                {
                    return "void";
                }
                default:
                {
                    return GetPrimitiveType(type);
                }
            }
        }
    }
}