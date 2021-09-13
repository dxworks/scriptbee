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
        
        public string GenerateClassName(string className, string superClassName)
        {
            return $"public class {className} : {superClassName}";
        }

        public string GenerateClassStart()
        {
            return "{";
        }

        public string GenerateClassEnd()
        {
            return "}";
        }

        public string GenerateField(string fieldModifier, Type fieldType, string fieldName)
        {
            var fieldTypeName = GetTypeName(fieldType);
            return $"    {fieldModifier} {fieldTypeName} {fieldName};";
        }

        public string GenerateProperty(string propertyModifier, Type propertyType, string propertyName)
        {
            var propertyTypeName = GetTypeName(propertyType);
            return $"    {propertyModifier} {propertyTypeName} {propertyName} {{ get; set; }}";
        }

        public string GenerateMethod(string methodModifier, Type methodType, string methodName, List<Tuple<string, string>> methodParams)
        {
            var stringBuilder = new StringBuilder();
            var methodTypeName = GetTypeName(methodType);
            stringBuilder.Append($"    {methodModifier} {methodTypeName} {methodName}(");
            
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

            if (methodTypeName != "void")
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
                case "System.Void":
                case "Void":
                {
                    return "void";
                }
            }

            return type;
        }

        private string GetTypeName(Type type)
        {
            return GetPrimitiveType(type.Name);
        }
    }
}