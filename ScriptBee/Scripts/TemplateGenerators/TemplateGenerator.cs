using System;
using System.Collections.Generic;
using System.Text;
using ScriptBee.Scripts.TemplateGenerators.Strategies;

namespace ScriptBee.Scripts.TemplateGenerators
{
    public class TemplateGenerator : ITemplateGenerator
    {
        private readonly IStrategyTemplateGenerator _strategyTemplateGenerator;

        private HashSet<string> _listedTypes = new HashSet<string>();

        public TemplateGenerator(IStrategyTemplateGenerator strategyTemplateGenerator)
        {
            _strategyTemplateGenerator = strategyTemplateGenerator;
        }

        public string Generate(Type type)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(GenerateClasses(type));

            stringBuilder.Append(GenerateSampleCode(type));

            return stringBuilder.ToString();
        }

        private string GenerateSampleCode(Type type)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateModelDeclaration(type.Name));

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateStartComment());

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateSampleCode());

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateEndComment());

            return stringBuilder.ToString();
        }

        private string GenerateClasses(Type type)
        {
            List<string> nonPrimitiveFieldResults = new List<string>();

            _listedTypes.Add(type.Name);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateClassName(type.Name));
            var classStart = _strategyTemplateGenerator.GenerateClassStart();
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

                stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateField(modifier,
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
                
                stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateField("public",
                    propertyInfo.PropertyType.Name,
                    propertyInfo.Name));
            }

            if (type.GetProperties().Length == 0 && type.GetFields().Length == 0)
            {
                stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateEmptyClass());
            }

            var classEnd = _strategyTemplateGenerator.GenerateClassEnd();
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