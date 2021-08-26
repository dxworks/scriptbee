using System;
using System.Collections.Generic;
using System.Text;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;

namespace ScriptBee.Scripts.ScriptSampleGenerators
{
    public class ScriptSampleGenerator : IScriptSampleGenerator
    {
        private readonly IStrategyGenerator _strategyGenerator;

        private readonly string _className = "ScriptContent";
        
        private readonly string _methodName = "ExecuteScript";

        private readonly HashSet<string> _listedTypes = new HashSet<string>();

        public ScriptSampleGenerator(IStrategyGenerator strategyGenerator)
        {
            _strategyGenerator = strategyGenerator;
        }

        public string Generate(Type type)
        {
            var stringBuilder = new StringBuilder();

            var imports = _strategyGenerator.GenerateImports();
            if (!string.IsNullOrEmpty(imports))
            {
                stringBuilder.AppendLine(imports);
                stringBuilder.AppendLine("");
            }

            stringBuilder.Append(GenerateClasses(type));

            stringBuilder.Append(GenerateSampleCode(type));

            return stringBuilder.ToString();
        }

        private string GenerateSampleCode(Type type)
        {
            var stringBuilder = new StringBuilder();

            var modelDeclaration = _strategyGenerator.GenerateModelDeclaration(type.Name);
            if (!string.IsNullOrEmpty(modelDeclaration))
            {
                stringBuilder.AppendLine(modelDeclaration);
                
                stringBuilder.AppendLine();
            }

            var sampleCode = _strategyGenerator.GenerateSampleCode();

            var finalSampleCode = sampleCode.Replace("$START_COMMENT$", _strategyGenerator.GetStartComment());
            
            finalSampleCode = finalSampleCode.Replace("$END_COMMENT$", _strategyGenerator.GetEndComment());

            finalSampleCode = finalSampleCode.Replace("$CLASS_NAME$", _className);
            
            finalSampleCode = finalSampleCode.Replace("$METHOD_NAME$", _methodName);
            
            finalSampleCode = finalSampleCode.Replace("$MODEL_TYPE$", type.Name);

            stringBuilder.AppendLine(finalSampleCode);
            
            return stringBuilder.ToString();
        }

        private string GenerateClasses(Type type)
        {
            List<string> nonPrimitiveFieldResults = new List<string>();

            _listedTypes.Add(type.Name);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(_strategyGenerator.GenerateClassName(type.Name));
            var classStart = _strategyGenerator.GenerateClassStart();
            if (!string.IsNullOrEmpty(classStart))
            {
                stringBuilder.AppendLine(classStart);
            }

            foreach (var fieldInfo in type.GetFields())
            {
                string modifier = "public";
                if (fieldInfo.IsPrivate)
                {
                    modifier = "private";
                }
                else if (fieldInfo.IsFamily)
                {
                    modifier = "protected";
                }

                if (!fieldInfo.FieldType.IsPrimitive && fieldInfo.FieldType.Name != "string" &&
                    fieldInfo.FieldType.Name != "System.String" && fieldInfo.FieldType.Name != "String")
                {
                    if (!_listedTypes.Contains(fieldInfo.FieldType.Name))
                    {
                        nonPrimitiveFieldResults.Add(GenerateClasses(fieldInfo.FieldType));
                    }
                }

                stringBuilder.AppendLine(_strategyGenerator.GenerateField(modifier,
                    fieldInfo.FieldType.Name,
                    fieldInfo.Name));
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType.Name != "string" &&
                    propertyInfo.PropertyType.Name != "System.String" && propertyInfo.PropertyType.Name != "String")
                {
                    if (!_listedTypes.Contains(propertyInfo.PropertyType.Name))
                    {
                        nonPrimitiveFieldResults.Add(GenerateClasses(propertyInfo.PropertyType));
                    }
                }

                stringBuilder.AppendLine(_strategyGenerator.GenerateField("public",
                    propertyInfo.PropertyType.Name,
                    propertyInfo.Name));
            }

            if (type.GetProperties().Length == 0 && type.GetFields().Length == 0)
            {
                stringBuilder.AppendLine(_strategyGenerator.GenerateEmptyClass());
            }

            var classEnd = _strategyGenerator.GenerateClassEnd();
            if (!string.IsNullOrEmpty(classEnd))
            {
                stringBuilder.AppendLine(classEnd);
            }

            stringBuilder.AppendLine();

            foreach (var result in nonPrimitiveFieldResults)
            {
                stringBuilder.Append(result);
            }

            return stringBuilder.ToString();
        }
    }
}