﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api;

public interface IScriptGeneratorStrategy : IPlugin
{
    // todo move in manifest.yaml
    public string Language { get; }
    public string Extension { get; }

    public string ExtractValidScript(string script);

    public string GenerateClassName(Type classType);

    public string GenerateClassName(Type classType, Type baseClassType, out HashSet<Type> baseClassGenericTypes);

    public string GenerateClassStart();

    public string GenerateClassEnd();

    public string GenerateField(string fieldModifier, Type fieldType, string fieldName,
        out HashSet<Type> genericTypes);

    public string GenerateProperty(string propertyModifier, Type propertyType, string propertyName,
        out HashSet<Type> genericTypes);

    public string GenerateMethod(string methodModifier, Type methodType, string methodName,
        List<Tuple<Type, string>> methodParams, out HashSet<Type> genericTypes);

    public string GenerateModelDeclaration(string modelType);

    public Task<string> GenerateSampleCode();

    public string GenerateEmptyClass();

    public Task<string> GenerateImports();

    public string GetStartComment();

    public string GetEndComment();
}
