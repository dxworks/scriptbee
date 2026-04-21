using System.Reflection;
using System.Text;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Common.CodeGeneration;

public class SampleCodeGenerator(
    IScriptGeneratorStrategy scriptGeneratorStrategy,
    ISet<string> acceptedModules
)
{
    private const BindingFlags BindingFlags =
        System.Reflection.BindingFlags.DeclaredOnly
        | System.Reflection.BindingFlags.Instance
        | System.Reflection.BindingFlags.Public;

    private readonly HashSet<string> _generatedClassNames = [];

    private const string ClassName = "ScriptContent";
    private const string MethodName = "ExecuteScript";
    private const string StartCommentMarker = "$START_COMMENT$";
    private const string EndCommentMarker = "$END_COMMENT$";
    private const string ClassNameMarker = "$CLASS_NAME$";
    private const string MethodNameMarker = "$METHOD_NAME$";
    private const string ModelTypeMarker = "$MODEL_TYPE$";

    public async Task<IList<SampleCodeFile>> GetSampleCode(
        IEnumerable<object> objects,
        CancellationToken cancellationToken = default
    )
    {
        var generatedClasses = new List<SampleCodeFile>();

        foreach (var obj in objects)
        {
            var type = obj.GetType();
            generatedClasses.AddRange(GenerateClasses(type));
        }

        return generatedClasses;
    }

    public async Task<string> GenerateSampleCode(CancellationToken cancellationToken = default)
    {
        var stringBuilder = new StringBuilder();
        var imports = await scriptGeneratorStrategy.GenerateImports();

        if (!string.IsNullOrEmpty(imports))
        {
            stringBuilder.AppendLine(imports);
            stringBuilder.AppendLine();
        }

        var modelDeclaration = scriptGeneratorStrategy.GenerateModelDeclaration(nameof(Project));
        if (!string.IsNullOrEmpty(modelDeclaration))
        {
            stringBuilder.AppendLine(modelDeclaration);

            stringBuilder.AppendLine();
        }

        var generateSampleCode = await scriptGeneratorStrategy.GenerateSampleCode();
        var sampleCode = ReplaceSampleCodeTemplates(nameof(Project), generateSampleCode);
        stringBuilder.Append(sampleCode);

        return stringBuilder.ToString();
    }

    private IList<SampleCodeFile> GenerateClasses(Type type)
    {
        if (
            _generatedClassNames.Contains(type.Name)
            || IsPrimitive(type)
            || !IsAcceptedModule(type.Module)
        )
        {
            return new List<SampleCodeFile>();
        }

        _generatedClassNames.Add(type.Name);

        var sampleCodeFiles = new List<SampleCodeFile>();

        var stringBuilder = new StringBuilder();

        var genericTypes = new HashSet<Type>();

        var baseType = type.BaseType;

        if (baseType != null && IsAcceptedModule(baseType.Module))
        {
            stringBuilder.AppendLine(
                scriptGeneratorStrategy.GenerateClassName(
                    type,
                    baseType,
                    out var baseClassGenericTypes
                )
            );

            foreach (var genericType in baseClassGenericTypes)
                genericTypes.Add(genericType);

            if (!_generatedClassNames.Contains(baseType.Name))
            {
                sampleCodeFiles.AddRange(GenerateClasses(baseType));
            }
        }
        else
        {
            stringBuilder.AppendLine(scriptGeneratorStrategy.GenerateClassName(type));
        }

        var classStart = scriptGeneratorStrategy.GenerateClassStart();
        if (!string.IsNullOrEmpty(classStart))
        {
            stringBuilder.AppendLine(classStart);
        }

        var fieldInfos = type.GetFields(BindingFlags);
        var propertyInfos = type.GetProperties(BindingFlags);

        if (!type.IsEnum)
        {
            foreach (var fieldInfo in fieldInfos)
            {
                var modifier = GetFieldModifier(fieldInfo);
                stringBuilder.AppendLine(
                    scriptGeneratorStrategy.GenerateField(
                        modifier,
                        fieldInfo.FieldType,
                        fieldInfo.Name,
                        out var receivedGenericTypes
                    )
                );

                foreach (var genericType in receivedGenericTypes)
                    genericTypes.Add(genericType);

                if (!_generatedClassNames.Contains(fieldInfo.FieldType.Name))
                {
                    sampleCodeFiles.AddRange(GenerateClasses(fieldInfo.FieldType));
                }
            }

            foreach (var propertyInfo in propertyInfos)
            {
                const string modifier = "public";
                stringBuilder.AppendLine(
                    scriptGeneratorStrategy.GenerateProperty(
                        modifier,
                        propertyInfo.PropertyType,
                        propertyInfo.Name,
                        out var receivedGenericTypes
                    )
                );

                foreach (var genericType in receivedGenericTypes)
                {
                    genericTypes.Add(genericType);
                }

                if (!_generatedClassNames.Contains(propertyInfo.PropertyType.Name))
                {
                    sampleCodeFiles.AddRange(GenerateClasses(propertyInfo.PropertyType));
                }
            }
        }

        var methodInfos = type.GetMethods(BindingFlags);

        foreach (var methodInfo in methodInfos)
        {
            if (methodInfo.IsSpecialName)
            {
                continue;
            }

            var modifier = GetMethodModifier(methodInfo);
            var methodParameters = methodInfo.GetParameters();

            var parameters = new List<Tuple<Type, string>>();

            foreach (var param in methodParameters)
            {
                parameters.Add(new Tuple<Type, string>(param.ParameterType, param.Name));
            }

            stringBuilder.AppendLine();

            stringBuilder.Append(
                scriptGeneratorStrategy.GenerateMethod(
                    modifier,
                    methodInfo.ReturnType,
                    methodInfo.Name,
                    parameters,
                    out var receivedGenericTypes
                )
            );

            foreach (var genericType in receivedGenericTypes)
                genericTypes.Add(genericType);
        }

        foreach (var genericType in genericTypes)
        {
            if (!_generatedClassNames.Contains(genericType.Name))
            {
                sampleCodeFiles.AddRange(GenerateClasses(genericType));
            }
        }

        var hasNoDeclaredMembers =
            (type.IsEnum || (fieldInfos.Length == 0 && propertyInfos.Length == 0))
            && methodInfos.Count(m => !m.IsSpecialName) == 0;

        if (hasNoDeclaredMembers)
        {
            stringBuilder.AppendLine(scriptGeneratorStrategy.GenerateEmptyClass());
        }

        var classEnd = scriptGeneratorStrategy.GenerateClassEnd();
        if (!string.IsNullOrEmpty(classEnd))
        {
            stringBuilder.AppendLine(classEnd);
        }

        sampleCodeFiles.Add(
            new SampleCodeFile { Name = type.Name, Content = stringBuilder.ToString() }
        );

        return sampleCodeFiles;
    }

    private string ReplaceSampleCodeTemplates(string modelName, string sampleCode)
    {
        var finalSampleCode = sampleCode.Replace(
            StartCommentMarker,
            scriptGeneratorStrategy.GetStartComment()
        );
        finalSampleCode = finalSampleCode.Replace(
            EndCommentMarker,
            scriptGeneratorStrategy.GetEndComment()
        );
        finalSampleCode = finalSampleCode.Replace(ClassNameMarker, ClassName);
        finalSampleCode = finalSampleCode.Replace(MethodNameMarker, MethodName);
        finalSampleCode = finalSampleCode.Replace(ModelTypeMarker, modelName);
        return finalSampleCode;
    }

    // todo extract in strategy
    private bool IsPrimitive(Type type)
    {
        return type.IsPrimitive || type.Name is "string" or "System.String" or "String";
    }

    // todo extract in strategy
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

    // todo extract in strategy
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
        return acceptedModules.Contains(module.Name);
    }
}
