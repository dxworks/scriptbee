using System;
using System.Text;
using ScriptBee.Scripts.TemplateGenerators.Strategies;

namespace ScriptBee.Scripts.TemplateGenerators
{
    public class TemplateGenerator : ITemplateGenerator
    {
        private readonly IStrategyTemplateGenerator _strategyTemplateGenerator;

        public TemplateGenerator(IStrategyTemplateGenerator strategyTemplateGenerator)
        {
            _strategyTemplateGenerator = strategyTemplateGenerator;
        }

        public string Generate(Type type)
        {
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

                stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateField(modifier,
                    fieldInfo.FieldType.ToString(),
                    fieldInfo.Name));
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateField("public",
                    propertyInfo.PropertyType.ToString(),
                    propertyInfo.Name));
            }

            var classEnd = _strategyTemplateGenerator.GenerateClassEnd();
            if (!string.IsNullOrEmpty(classEnd))
            {
                stringBuilder.AppendLine(classEnd);
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateModelDeclaration(type.Name));

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateStartComment());

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateSampleCode());

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(_strategyTemplateGenerator.GenerateEndComment());

            return stringBuilder.ToString();
        }
    }
}