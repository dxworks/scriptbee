using ScriptBee.Utils;

namespace ScriptBee.Scripts.ScriptSampleGenerators.Strategies
{
    public class CSharpStrategyGenerator : IStrategyGenerator
    {
        private string _modelType;

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

        public string GenerateStartComment()
        {
            return ValidScriptDelimiters.CSharpStartComment;
        }

        public string GenerateEndComment()
        {
            return ValidScriptDelimiters.CSharpEndComment;
        }

        public string GenerateModelDeclaration(string modelType)
        {
            _modelType = modelType;
            return "";
        }

        public string GenerateSampleCode()
        {
            return $@"public class ScriptContent
{{
    public void ExecuteScript({_modelType} model)
    {{
        Console.WriteLine(model);
    }}
}}";
            // create instance, ins.invoke("ExecuteScript", new object[] {model});
        }

        public string GenerateEmptyClass()
        {
            return "";
        }

        public string GenerateImports()
        {
            return "using System;";
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
                    return "boolean";
                }
            }

            return type;
        }
    }
}