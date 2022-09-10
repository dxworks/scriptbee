using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;

namespace ScriptBee.Tests.Plugin.Internals;

internal class TestScriptGeneratorPlugin : IScriptGeneratorStrategy
{
    public string Language { get; } = "";
    public string Extension { get; } = "";

    public string GenerateClassName(Type classType)
    {
        return "";
    }

    public string GenerateClassName(Type classType, Type baseClassType, out HashSet<Type> baseClassGenericTypes)
    {
        baseClassGenericTypes = new HashSet<Type>();
        return "";
    }

    public string GenerateClassStart()
    {
        return "";
    }

    public string GenerateClassEnd()
    {
        return "";
    }

    public string GenerateField(string fieldModifier, Type fieldType, string fieldName, out HashSet<Type> genericTypes)
    {
        genericTypes = new HashSet<Type>();
        return "";
    }

    public string GenerateProperty(string propertyModifier, Type propertyType, string propertyName,
        out HashSet<Type> genericTypes)
    {
        genericTypes = new HashSet<Type>();
        return "";
    }

    public string GenerateMethod(string methodModifier, Type methodType, string methodName,
        List<Tuple<Type, string>> methodParams,
        out HashSet<Type> genericTypes)
    {
        genericTypes = new HashSet<Type>();
        return "";
    }

    public string GenerateModelDeclaration(string modelType)
    {
        return "";
    }

    public Task<string> GenerateSampleCode()
    {
        return Task.FromResult("");
    }

    public string GenerateEmptyClass()
    {
        return "";
    }

    public Task<string> GenerateImports()
    {
        return Task.FromResult("");
    }

    public string GetStartComment()
    {
        return "";
    }

    public string GetEndComment()
    {
        return "";
    }
}
