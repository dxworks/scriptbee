using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.ProjectContext;

namespace ScriptBee.Scripts.ScriptSampleGenerators;

public class SampleCodeGenerator
{
    private const BindingFlags BindingFlags = System.Reflection.BindingFlags.DeclaredOnly |
                                              System.Reflection.BindingFlags.Instance |
                                              System.Reflection.BindingFlags.Public;

    private const string ClassName = "ScriptContent";

    private const string MethodName = "ExecuteScript";
    private readonly ISet<string> _acceptedModules;
    private readonly HashSet<string> _generatedClassNames = new();
    private readonly IScriptGeneratorStrategy _scriptGeneratorStrategy;

    public SampleCodeGenerator(IScriptGeneratorStrategy scriptGeneratorStrategy, ISet<string> acceptedModules)
    {
        _scriptGeneratorStrategy = scriptGeneratorStrategy;
        _acceptedModules = acceptedModules;
    }

    public async Task<IList<SampleCodeFile>> GetSampleCode(IEnumerable<object> objects,
        CancellationToken cancellationToken = default)
    {
        var generatedClasses = new List<SampleCodeFile>();

        foreach (var obj in objects)
        {
            var type = obj.GetType();
            generatedClasses.AddRange(GenerateClasses(type));
        }

        var generateSampleCode = await GenerateSampleCode(cancellationToken);
        generatedClasses.Add(new SampleCodeFile
        {
            Name = "script",
            Content = generateSampleCode
        });

        return generatedClasses;
    }

    public async Task<string> GenerateSampleCode(CancellationToken cancellationToken = default)
    {
        var stringBuilder = new StringBuilder();
        var imports = await _scriptGeneratorStrategy.GenerateImports();

        if (!string.IsNullOrEmpty(imports))
        {
            stringBuilder.AppendLine(imports);
            stringBuilder.AppendLine();
        }

        var modelDeclaration = _scriptGeneratorStrategy.GenerateModelDeclaration(nameof(Project));
        if (!string.IsNullOrEmpty(modelDeclaration))
        {
            stringBuilder.AppendLine(modelDeclaration);

            stringBuilder.AppendLine();
        }

        var generateSampleCode = await _scriptGeneratorStrategy.GenerateSampleCode();
        var sampleCode = ReplaceSampleCodeTemplates(nameof(Project), generateSampleCode);
        stringBuilder.Append(sampleCode);

        return stringBuilder.ToString();
    }

    private IList<SampleCodeFile> GenerateClasses(Type type)
    {
        if (_generatedClassNames.Contains(type.Name) || IsPrimitive(type) || !IsAcceptedModule(type.Module))
            return new List<SampleCodeFile>();

        _generatedClassNames.Add(type.Name);

        var sampleCodeFiles = new List<SampleCodeFile>();

        var stringBuilder = new StringBuilder();

        var genericTypes = new HashSet<Type>();

        var baseType = type.BaseType;

        if (baseType != null && IsAcceptedModule(baseType.Module))
        {
            stringBuilder.AppendLine(
                _scriptGeneratorStrategy.GenerateClassName(type, baseType, out var baseClassGenericTypes));

            foreach (var genericType in baseClassGenericTypes) genericTypes.Add(genericType);

            if (!_generatedClassNames.Contains(baseType.Name)) sampleCodeFiles.AddRange(GenerateClasses(baseType));
        }
        else
        {
            stringBuilder.AppendLine(_scriptGeneratorStrategy.GenerateClassName(type));
        }

        var classStart = _scriptGeneratorStrategy.GenerateClassStart();
        if (!string.IsNullOrEmpty(classStart)) stringBuilder.AppendLine(classStart);

        foreach (var fieldInfo in type.GetFields(BindingFlags))
        {
            var modifier = GetFieldModifier(fieldInfo);
            stringBuilder.AppendLine(_scriptGeneratorStrategy.GenerateField(modifier, fieldInfo.FieldType,
                fieldInfo.Name, out var receivedGenericTypes));

            foreach (var genericType in receivedGenericTypes) genericTypes.Add(genericType);

            if (!_generatedClassNames.Contains(fieldInfo.FieldType.Name))
                sampleCodeFiles.AddRange(GenerateClasses(fieldInfo.FieldType));
        }

        foreach (var propertyInfo in type.GetProperties(BindingFlags))
        {
            const string modifier = "public";
            stringBuilder.AppendLine(_scriptGeneratorStrategy.GenerateProperty(modifier, propertyInfo.PropertyType,
                propertyInfo.Name, out var receivedGenericTypes));

            foreach (var genericType in receivedGenericTypes) genericTypes.Add(genericType);

            if (!_generatedClassNames.Contains(propertyInfo.PropertyType.Name))
                sampleCodeFiles.AddRange(GenerateClasses(propertyInfo.PropertyType));
        }

        foreach (var methodInfo in type.GetMethods(BindingFlags))
        {
            if (methodInfo.IsSpecialName) continue;

            var modifier = GetMethodModifier(methodInfo);
            var methodParameters = methodInfo.GetParameters();

            var parameters = new List<Tuple<Type, string>>();

            foreach (var param in methodParameters)
                parameters.Add(new Tuple<Type, string>(param.ParameterType, param.Name));

            stringBuilder.AppendLine();

            stringBuilder.Append(_scriptGeneratorStrategy.GenerateMethod(modifier, methodInfo.ReturnType,
                methodInfo.Name, parameters, out var receivedGenericTypes));

            foreach (var genericType in receivedGenericTypes) genericTypes.Add(genericType);
        }

        foreach (var genericType in genericTypes)
            if (!_generatedClassNames.Contains(genericType.Name))
                sampleCodeFiles.AddRange(GenerateClasses(genericType));

        // 4 = default object methods
        if (type.GetProperties().Length == 0 && type.GetFields().Length == 0 && type.GetMethods().Length <= 4)
            stringBuilder.AppendLine(_scriptGeneratorStrategy.GenerateEmptyClass());

        var classEnd = _scriptGeneratorStrategy.GenerateClassEnd();
        if (!string.IsNullOrEmpty(classEnd)) stringBuilder.AppendLine(classEnd);

        sampleCodeFiles.Add(new SampleCodeFile
        {
            Name = type.Name,
            Content = stringBuilder.ToString()
        });

        return sampleCodeFiles;
    }

    // todo extract hardcoded strings as constants
    private string ReplaceSampleCodeTemplates(string modelName, string sampleCode)
    {
        var finalSampleCode = sampleCode.Replace("$START_COMMENT$", _scriptGeneratorStrategy.GetStartComment());

        finalSampleCode = finalSampleCode.Replace("$END_COMMENT$", _scriptGeneratorStrategy.GetEndComment());

        finalSampleCode = finalSampleCode.Replace("$CLASS_NAME$", ClassName);

        finalSampleCode = finalSampleCode.Replace("$METHOD_NAME$", MethodName);

        finalSampleCode = finalSampleCode.Replace("$MODEL_TYPE$", modelName);
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
            modifier = "private";
        else if (fieldInfo.IsFamily) modifier = "protected";

        return modifier;
    }

    // todo extract in strategy
    private string GetMethodModifier(MethodInfo methodInfo)
    {
        var modifier = "public";
        if (methodInfo.IsPrivate)
            modifier = "private";
        else if (methodInfo.IsFamily) modifier = "protected";

        return modifier;
    }

    private bool IsAcceptedModule(Module module)
    {
        return _acceptedModules.Contains(module.Name);
    }
}
