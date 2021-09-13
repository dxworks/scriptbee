using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;

namespace ScriptBee.Scripts.ScriptSampleGenerators
{
    public class SampleCodeGenerator : ISampleCodeGenerator
    {
        private readonly IStrategyGenerator _strategyGenerator;
        private readonly ISet<string> _generatedClassNames = new HashSet<string>();
        private readonly HashSet<string> _acceptedModules = new HashSet<string>();

        private readonly BindingFlags _bindingFlags =
            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

        private const string ClassName = "ScriptContent";

        private const string MethodName = "ExecuteScript";

        public SampleCodeGenerator(IStrategyGenerator strategyGenerator, ILoadersHolder loadersHolder)
        {
            _strategyGenerator = strategyGenerator;

            foreach (var modelLoader in loadersHolder.GetAllLoaders())
            {
                _acceptedModules.Add(modelLoader.GetType().Module.Name);
            }
        }

        public IList<SampleCodeFile> GetSampleCode(object obj)
        {
            var type = obj.GetType();
            var generatedClasses = GenerateClasses(type);

            generatedClasses.Add(new SampleCodeFile
            {
                Name = "script",
                Content = GenerateSampleCode()
            });

            return generatedClasses;
        }

        public IList<SampleCodeFile> GetSampleCode(IEnumerable<object> objects)
        {
            var generatedClasses = new List<SampleCodeFile>();

            foreach (var obj in objects)
            {
                var type = obj.GetType();
                generatedClasses.AddRange(GenerateClasses(type));
            }

            generatedClasses.Add(new SampleCodeFile
            {
                Name = "script",
                Content = GenerateSampleCode()
            });

            return generatedClasses;
        }

        private string GenerateSampleCode()
        {
            var stringBuilder = new StringBuilder();
            var imports = _strategyGenerator.GenerateImports();

            if (!string.IsNullOrEmpty(imports))
            {
                stringBuilder.AppendLine(imports);
                stringBuilder.AppendLine();
            }

            var modelDeclaration = _strategyGenerator.GenerateModelDeclaration(nameof(Project));
            if (!string.IsNullOrEmpty(modelDeclaration))
            {
                stringBuilder.AppendLine(modelDeclaration);

                stringBuilder.AppendLine();
            }

            var sampleCode = ReplaceSampleCodeTemplates(nameof(Project), _strategyGenerator.GenerateSampleCode());
            stringBuilder.AppendLine(sampleCode);

            return stringBuilder.ToString();
        }

        private IList<SampleCodeFile> GenerateClasses(Type type)
        {
            if (_generatedClassNames.Contains(type.Name))
            {
                return new List<SampleCodeFile>();
            }

            _generatedClassNames.Add(type.Name);

            var sampleCodeFiles = new List<SampleCodeFile>();
            
            var stringBuilder = new StringBuilder();

            var baseType = type.BaseType;

            if (baseType != null && IsAcceptedModule(baseType.Module))
            {
                stringBuilder.AppendLine(_strategyGenerator.GenerateClassName(type.Name, baseType.Name));
                
                if (!_generatedClassNames.Contains(baseType.Name))
                {
                    sampleCodeFiles.AddRange(GenerateClasses(baseType));
                }
            }
            else
            {
                stringBuilder.AppendLine(_strategyGenerator.GenerateClassName(type.Name));
            }
            
            var classStart = _strategyGenerator.GenerateClassStart();
            if (!string.IsNullOrEmpty(classStart))
            {
                stringBuilder.AppendLine(classStart);
            }

            foreach (var fieldInfo in type.GetFields(_bindingFlags))
            {
                if (!IsAcceptedModule(fieldInfo.Module))
                {
                    continue;
                }

                var modifier = GetFieldModifier(fieldInfo);
                stringBuilder.AppendLine(_strategyGenerator.GenerateField(modifier, fieldInfo.FieldType,
                    fieldInfo.Name));
                if (IsPrimitive(fieldInfo.FieldType))
                {
                    continue;
                }

                if (!_generatedClassNames.Contains(fieldInfo.FieldType.Name))
                {
                    sampleCodeFiles.AddRange(GenerateClasses(fieldInfo.FieldType));
                }
            }

            foreach (var propertyInfo in type.GetProperties(_bindingFlags))
            {
                if (!IsAcceptedModule(propertyInfo.Module))
                {
                    continue;
                }

                const string modifier = "public";
                stringBuilder.AppendLine(_strategyGenerator.GenerateProperty(modifier, propertyInfo.PropertyType,
                    propertyInfo.Name));
                if (IsPrimitive(propertyInfo.PropertyType))
                {
                    continue;
                }

                if (!_generatedClassNames.Contains(propertyInfo.PropertyType.Name))
                {
                    sampleCodeFiles.AddRange(GenerateClasses(propertyInfo.PropertyType));
                }
            }

            foreach (var methodInfo in type.GetMethods(_bindingFlags))
            {
                if (methodInfo.IsSpecialName || !IsAcceptedModule(methodInfo.Module))
                {
                    continue;
                }

                var modifier = GetMethodModifier(methodInfo);
                var methodParameters = methodInfo.GetParameters();

                List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

                foreach (var param in methodParameters)
                {
                    parameters.Add(new Tuple<string, string>(param.ParameterType.Name, param.Name));
                }

                stringBuilder.AppendLine();

                stringBuilder.Append(_strategyGenerator.GenerateMethod(modifier, methodInfo.ReturnType,
                    methodInfo.Name, parameters));
            }

            // 4 = default object methods
            if (type.GetProperties().Length == 0 && type.GetFields().Length == 0 && type.GetMethods().Length <= 4)
            {
                stringBuilder.AppendLine(_strategyGenerator.GenerateEmptyClass());
            }

            var classEnd = _strategyGenerator.GenerateClassEnd();
            if (!string.IsNullOrEmpty(classEnd))
            {
                stringBuilder.AppendLine(classEnd);
            }

            sampleCodeFiles.Add(new SampleCodeFile
            {
                Name = type.Name,
                Content = stringBuilder.ToString()
            });

            return sampleCodeFiles;
        }

        private string ReplaceSampleCodeTemplates(string modelName, string sampleCode)
        {
            var finalSampleCode = sampleCode.Replace("$START_COMMENT$", _strategyGenerator.GetStartComment());

            finalSampleCode = finalSampleCode.Replace("$END_COMMENT$", _strategyGenerator.GetEndComment());

            finalSampleCode = finalSampleCode.Replace("$CLASS_NAME$", ClassName);

            finalSampleCode = finalSampleCode.Replace("$METHOD_NAME$", MethodName);

            finalSampleCode = finalSampleCode.Replace("$MODEL_TYPE$", modelName);
            return finalSampleCode;
        }

        private bool IsPrimitive(Type type)
        {
            return type.IsPrimitive || type.Name is "string" or "System.String" or "String";
        }

        private string GetFieldModifier(FieldInfo fieldInfo)
        {
            var modifier = "public";
            if (fieldInfo.IsPrivate)
            {
                modifier = "private";
            }
            else if (fieldInfo.IsFamily)
            {
                modifier = "protected";
            }

            return modifier;
        }

        private string GetMethodModifier(MethodInfo methodInfo)
        {
            var modifier = "public";
            if (methodInfo.IsPrivate)
            {
                modifier = "private";
            }
            else if (methodInfo.IsFamily)
            {
                modifier = "protected";
            }

            return modifier;
        }

        private bool IsAcceptedModule(Module module)
        {
            return _acceptedModules.Contains(module.Name);
        }
    }
}