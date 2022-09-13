﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.ScriptGeneration.TestsCommon;
using ScriptBee.Scripts.ScriptSampleGenerators;
using Xunit;

namespace DxWorks.ScriptBee.Plugin.ScriptGeneration.CSharp.Tests;

public class ScriptGeneratorStrategyTests
{
    private readonly SampleCodeGenerator _sampleCodeGenerator;

    public ScriptGeneratorStrategyTests()
    {
        _sampleCodeGenerator = new SampleCodeGenerator(new ScriptGeneratorStrategy(), new HashSet<string>
        {
            new TestModelLoader().GetType().Module.Name
        });
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/CSharpDummyModel.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDummyModel(string pathToModel,
        string pathToSampleCode)
    {
        var modelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);
        var sampleCode = await _sampleCodeGenerator.GetSampleCode(new List<object> { new DummyModel() });

        Assert.Equal(2, sampleCode.Count);

        Assert.Equal(modelContent, sampleCode[0].Content);
        Assert.Equal(sampleCodeContent, sampleCode[1].Content);

        Assert.Equal("DummyModel", sampleCode[0].Name);
        Assert.Equal("script", sampleCode[1].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/CSharpDummyModel.txt",
        "ScriptSampleTestStrings/RecursiveModel/CSharpRecursiveModel_RecursiveModel.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task GenerateSampleCode_MainModelGivenAsObject_ShouldReturnRecursiveModel(
        string pathToDummyModel, string pathToMainModel, string pathToSampleCode)
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToDummyModel);
        var mainModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToMainModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(new List<object> { new RecursiveModel() });

        Assert.Equal(3, sampleCode.Count);

        Assert.Equal(dummyModelContent, sampleCode[0].Content);
        Assert.Equal(mainModelContent, sampleCode[1].Content);
        Assert.Equal(sampleCodeContent, sampleCode[2].Content);

        Assert.Equal("DummyModel", sampleCode[0].Name);
        Assert.Equal("RecursiveModel", sampleCode[1].Name);
        Assert.Equal("script", sampleCode[2].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/CSharpDummyModel.txt",
        "ScriptSampleTestStrings/DummyModelInheritor/CSharpDummyModelInheritor_DummyModelInheritor.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDummyModelInheritor(
        string pathToDummyModel, string pathToMainModel, string pathToSampleCode)
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToDummyModel);
        var mainModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToMainModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(new List<object> { new DummyModelInheritor() });

        Assert.Equal(3, sampleCode.Count);

        Assert.Equal(dummyModelContent, sampleCode[0].Content);
        Assert.Equal(mainModelContent, sampleCode[1].Content);
        Assert.Equal(sampleCodeContent, sampleCode[2].Content);

        Assert.Equal("DummyModel", sampleCode[0].Name);
        Assert.Equal("DummyModelInheritor", sampleCode[1].Name);
        Assert.Equal("script", sampleCode[2].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/CSharpDummyModel.txt",
        "ScriptSampleTestStrings/RecursiveModel/CSharpRecursiveModel_RecursiveModel.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task GenerateSampleCode_ModelsGivenAsListOfObjects_ShouldReturnRecursiveModel(
        string pathToDummyModel, string pathToMainModel, string pathToSampleCode)
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToDummyModel);
        var mainModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToMainModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);

        var models = new List<object>
        {
            new DummyModel(),
            new RecursiveModel()
        };

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(models);

        Assert.Equal(3, sampleCode.Count);

        Assert.Equal(dummyModelContent, sampleCode[0].Content);
        Assert.Equal(mainModelContent, sampleCode[1].Content);
        Assert.Equal(sampleCodeContent, sampleCode[2].Content);

        Assert.Equal("DummyModel", sampleCode[0].Name);
        Assert.Equal("RecursiveModel", sampleCode[1].Name);
        Assert.Equal("script", sampleCode[2].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/CSharpDummyModel.txt",
        "ScriptSampleTestStrings/DeepModel/CSharpDeepModel_EmptyModel.txt",
        "ScriptSampleTestStrings/DeepModel/CSharpDeepModel_RecursiveModel.txt",
        "ScriptSampleTestStrings/DeepModel/CSharpDeepModel_RecursiveModel2.txt",
        "ScriptSampleTestStrings/DeepModel/CSharpDeepModel_DeepModel.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDeepModel(string pathToDummyModel,
        string pathToEmptyModel, string pathToRecursiveModel, string pathToRecursiveModel2, string pathToMainModel,
        string pathToSampleCode)
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToDummyModel);
        var emptyModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToEmptyModel);
        var recursiveModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToRecursiveModel);
        var recursiveModel2Content = await RelativeFileContentProvider.GetFileContentAsync(pathToRecursiveModel2);
        var deepModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToMainModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(new List<object> { new DeepModel() });

        Assert.Equal(6, sampleCode.Count);

        Assert.Equal(dummyModelContent, sampleCode[0].Content);
        Assert.Equal(recursiveModelContent, sampleCode[1].Content);
        Assert.Equal(recursiveModel2Content, sampleCode[2].Content);
        Assert.Equal(emptyModelContent, sampleCode[3].Content);
        Assert.Equal(deepModelContent, sampleCode[4].Content);
        Assert.Equal(sampleCodeContent, sampleCode[5].Content);

        Assert.Equal("DummyModel", sampleCode[0].Name);
        Assert.Equal("RecursiveModel", sampleCode[1].Name);
        Assert.Equal("RecursiveModel2", sampleCode[2].Name);
        Assert.Equal("EmptyModel", sampleCode[3].Name);
        Assert.Equal("DeepModel", sampleCode[4].Name);
        Assert.Equal("script", sampleCode[5].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/CSharpDummyModel.txt",
        "ScriptSampleTestStrings/DeepModel/CSharpDeepModel_EmptyModel.txt",
        "ScriptSampleTestStrings/DeepModel/CSharpDeepModel_RecursiveModel.txt",
        "ScriptSampleTestStrings/DeepModel/CSharpDeepModel_RecursiveModel2.txt",
        "ScriptSampleTestStrings/DeepModel/CSharpDeepModel_DeepModel.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task GenerateSampleCode_ModelsGivenAsListOfObjects_ShouldReturnDeepModel(string pathToDummyModel,
        string pathToEmptyModel, string pathToRecursiveModel, string pathToRecursiveModel2, string pathToMainModel,
        string pathToSampleCode)
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToDummyModel);
        var emptyModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToEmptyModel);
        var recursiveModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToRecursiveModel);
        var recursiveModel2Content = await RelativeFileContentProvider.GetFileContentAsync(pathToRecursiveModel2);
        var deepModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToMainModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);

        var models = new List<object>
        {
            new DummyModel(),
            new RecursiveModel(),
            new RecursiveModel2(),
            new EmptyModel(),
            new DeepModel()
        };

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(models);

        Assert.Equal(6, sampleCode.Count);

        Assert.Equal(dummyModelContent, sampleCode[0].Content);
        Assert.Equal(recursiveModelContent, sampleCode[1].Content);
        Assert.Equal(recursiveModel2Content, sampleCode[2].Content);
        Assert.Equal(emptyModelContent, sampleCode[3].Content);
        Assert.Equal(deepModelContent, sampleCode[4].Content);
        Assert.Equal(sampleCodeContent, sampleCode[5].Content);

        Assert.Equal("DummyModel", sampleCode[0].Name);
        Assert.Equal("RecursiveModel", sampleCode[1].Name);
        Assert.Equal("RecursiveModel2", sampleCode[2].Name);
        Assert.Equal("EmptyModel", sampleCode[3].Name);
        Assert.Equal("DeepModel", sampleCode[4].Name);
        Assert.Equal("script", sampleCode[5].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/DummyModelWithMethods/CSharpDummyModel_WithMethods.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task GenerateSampleCode_DummyModelWithMethods_ShouldReturnDummyModelWithMethods(string pathToModel,
        string pathToSampleCode)
    {
        var modelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(new List<object> { new DummyModelWithMethods() });

        Assert.Equal(2, sampleCode.Count);

        Assert.Equal(modelContent, sampleCode[0].Content);
        Assert.Equal(sampleCodeContent, sampleCode[1].Content);

        Assert.Equal("DummyModelWithMethods", sampleCode[0].Name);
        Assert.Equal("script", sampleCode[1].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/DummyModelWithMethods/CSharpDummyModel_WithMethods_Expando.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task
        GenerateSampleCode_WithCSharpStrategy_DummyModelWithMethodsExpando_ShouldReturnCSharpSimpleModelWithMethods(
            string pathToModel, string pathToSampleCode)
    {
        var modelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);

        var sampleCode =
            await _sampleCodeGenerator.GetSampleCode(new List<object> { new DummyModelWithMethodsExpando() });

        Assert.Equal(2, sampleCode.Count);

        Assert.Equal(modelContent, sampleCode[0].Content);
        Assert.Equal(sampleCodeContent, sampleCode[1].Content);

        Assert.Equal("DummyModelWithMethodsExpando", sampleCode[0].Name);
        Assert.Equal("script", sampleCode[1].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/DummyModelWithMethods/CSharpDummyModel_WithMethods.txt",
        "ScriptSampleTestStrings/CSharpDummyModel.txt",
        "ScriptSampleTestStrings/GenericModel/CSharpGenericModel_GenericModel.txt",
        "ScriptSampleTestStrings/GenericModel/CSharpGenericModel_GenericModel2.txt",
        "ScriptSampleTestStrings/GenericModel/CSharpGenericModel_NestedGenericModel.txt",
        "ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt")]
    public async Task GenerateSampleCode_NestedGenericModel_ModelGivenAsObject_ShouldReturnGenericModel_CSharp(
        string pathToDummyModel, string pathToDummyModelWithMethods, string pathToGenericModel,
        string pathToGenericModel2,
        string pathToNestedGenericModel, string pathToSampleCode)
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToDummyModel);
        var dummyModelWithMethodsContent =
            await RelativeFileContentProvider.GetFileContentAsync(pathToDummyModelWithMethods);
        var genericModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToGenericModel);
        var genericModel2Content = await RelativeFileContentProvider.GetFileContentAsync(pathToGenericModel2);
        var nestedGenericModelContent = await RelativeFileContentProvider.GetFileContentAsync(pathToNestedGenericModel);
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(pathToSampleCode);


        var sampleCode = await _sampleCodeGenerator.GetSampleCode(new List<object> { new NestedGenericModel() });

        Assert.Equal(6, sampleCode.Count);

        Assert.Equal(genericModelContent, sampleCode[0].Content);
        Assert.Equal(genericModel2Content, sampleCode[1].Content);
        Assert.Equal(dummyModelWithMethodsContent, sampleCode[2].Content);
        Assert.Equal(dummyModelContent, sampleCode[3].Content);
        Assert.Equal(nestedGenericModelContent, sampleCode[4].Content);
        Assert.Equal(sampleCodeContent, sampleCode[5].Content);
    }
}
