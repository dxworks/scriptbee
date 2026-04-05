using DxWorks.ScriptBee.Plugin.ScriptRunner.TestsCommon;
using ScriptBee.Common.CodeGeneration;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript.Tests;

public class ScriptGeneratorTests
{
    private readonly SampleCodeGenerator _sampleCodeGenerator = new(
        new ScriptGeneratorStrategy(),
        new HashSet<string> { new TestModelLoader().GetType().Module.Name }
    );

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/JavascriptDummyModel.txt",
        "ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt"
    )]
    public async Task GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDummyModel(
        string pathToModel,
        string pathToSampleCode
    )
    {
        var modelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToModel,
            TestContext.Current.CancellationToken
        );
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToSampleCode,
            TestContext.Current.CancellationToken
        );

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(
            new List<object> { new DummyModel() },
            TestContext.Current.CancellationToken
        );

        Assert.Equal(2, sampleCode.Count);

        Assert.Equal(modelContent, sampleCode[0].Content);
        Assert.Equal(sampleCodeContent, sampleCode[1].Content);

        Assert.Equal("DummyModel", sampleCode[0].Name);
        Assert.Equal("script", sampleCode[1].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/JavascriptDummyModel.txt",
        "ScriptSampleTestStrings/RecursiveModel/JavascriptRecursiveModel_RecursiveModel.txt",
        "ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt"
    )]
    public async Task GenerateSampleCode_MainModelGivenAsObject_ShouldReturnRecursiveModel(
        string pathToDummyModel,
        string pathToMainModel,
        string pathToSampleCode
    )
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToDummyModel,
            TestContext.Current.CancellationToken
        );
        var mainModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToMainModel,
            TestContext.Current.CancellationToken
        );
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToSampleCode,
            TestContext.Current.CancellationToken
        );

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(
            new List<object> { new RecursiveModel() },
            TestContext.Current.CancellationToken
        );

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
        "ScriptSampleTestStrings/JavascriptDummyModel.txt",
        "ScriptSampleTestStrings/DummyModelInheritor/JavascriptDummyModelInheritor_DummyModelInheritor.txt",
        "ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt"
    )]
    public async Task GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDummyModelInheritor(
        string pathToDummyModel,
        string pathToMainModel,
        string pathToSampleCode
    )
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToDummyModel,
            TestContext.Current.CancellationToken
        );
        var mainModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToMainModel,
            TestContext.Current.CancellationToken
        );
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToSampleCode,
            TestContext.Current.CancellationToken
        );

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(
            new List<object> { new DummyModelInheritor() },
            TestContext.Current.CancellationToken
        );

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
        "ScriptSampleTestStrings/JavascriptDummyModel.txt",
        "ScriptSampleTestStrings/RecursiveModel/JavascriptRecursiveModel_RecursiveModel.txt",
        "ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt"
    )]
    public async Task GenerateSampleCode_ModelsGivenAsListOfObjects_ShouldReturnRecursiveModel(
        string pathToDummyModel,
        string pathToMainModel,
        string pathToSampleCode
    )
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToDummyModel,
            TestContext.Current.CancellationToken
        );
        var mainModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToMainModel,
            TestContext.Current.CancellationToken
        );
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToSampleCode,
            TestContext.Current.CancellationToken
        );

        var models = new List<object> { new DummyModel(), new RecursiveModel() };

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(
            models,
            TestContext.Current.CancellationToken
        );

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
        "ScriptSampleTestStrings/JavascriptDummyModel.txt",
        "ScriptSampleTestStrings/DeepModel/JavascriptDeepModel_EmptyModel.txt",
        "ScriptSampleTestStrings/DeepModel/JavascriptDeepModel_RecursiveModel.txt",
        "ScriptSampleTestStrings/DeepModel/JavascriptDeepModel_RecursiveModel2.txt",
        "ScriptSampleTestStrings/DeepModel/JavascriptDeepModel_DeepModel.txt",
        "ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt"
    )]
    public async Task GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDeepModel(
        string pathToDummyModel,
        string pathToEmptyModel,
        string pathToRecursiveModel,
        string pathToRecursiveModel2,
        string pathToMainModel,
        string pathToSampleCode
    )
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToDummyModel,
            TestContext.Current.CancellationToken
        );
        var emptyModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToEmptyModel,
            TestContext.Current.CancellationToken
        );
        var recursiveModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToRecursiveModel,
            TestContext.Current.CancellationToken
        );
        var recursiveModel2Content = await RelativeFileContentProvider.GetFileContentAsync(
            pathToRecursiveModel2,
            TestContext.Current.CancellationToken
        );
        var deepModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToMainModel,
            TestContext.Current.CancellationToken
        );
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToSampleCode,
            TestContext.Current.CancellationToken
        );

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(
            new List<object> { new DeepModel() },
            TestContext.Current.CancellationToken
        );

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
        "ScriptSampleTestStrings/JavascriptDummyModel.txt",
        "ScriptSampleTestStrings/DeepModel/JavascriptDeepModel_EmptyModel.txt",
        "ScriptSampleTestStrings/DeepModel/JavascriptDeepModel_RecursiveModel.txt",
        "ScriptSampleTestStrings/DeepModel/JavascriptDeepModel_RecursiveModel2.txt",
        "ScriptSampleTestStrings/DeepModel/JavascriptDeepModel_DeepModel.txt",
        "ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt"
    )]
    public async Task GenerateSampleCode_ModelsGivenAsListOfObjects_ShouldReturnDeepModel(
        string pathToDummyModel,
        string pathToEmptyModel,
        string pathToRecursiveModel,
        string pathToRecursiveModel2,
        string pathToMainModel,
        string pathToSampleCode
    )
    {
        var dummyModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToDummyModel,
            TestContext.Current.CancellationToken
        );
        var emptyModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToEmptyModel,
            TestContext.Current.CancellationToken
        );
        var recursiveModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToRecursiveModel,
            TestContext.Current.CancellationToken
        );
        var recursiveModel2Content = await RelativeFileContentProvider.GetFileContentAsync(
            pathToRecursiveModel2,
            TestContext.Current.CancellationToken
        );
        var deepModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToMainModel,
            TestContext.Current.CancellationToken
        );
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToSampleCode,
            TestContext.Current.CancellationToken
        );

        var models = new List<object>
        {
            new DummyModel(),
            new RecursiveModel(),
            new RecursiveModel2(),
            new EmptyModel(),
            new DeepModel(),
        };

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(
            models,
            TestContext.Current.CancellationToken
        );

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
        "ScriptSampleTestStrings/DummyModelWithMethods/JavascriptDummyModel_WithMethods.txt",
        "ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt"
    )]
    public async Task GenerateSampleCode_DummyModelWithMethods_ShouldReturnDummyModelWithMethods(
        string pathToModel,
        string pathToSampleCode
    )
    {
        var modelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToModel,
            TestContext.Current.CancellationToken
        );
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToSampleCode,
            TestContext.Current.CancellationToken
        );

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(
            new List<object> { new DummyModelWithMethods() },
            TestContext.Current.CancellationToken
        );

        Assert.Equal(2, sampleCode.Count);

        Assert.Equal(modelContent, sampleCode[0].Content);
        Assert.Equal(sampleCodeContent, sampleCode[1].Content);

        Assert.Equal("DummyModelWithMethods", sampleCode[0].Name);
        Assert.Equal("script", sampleCode[1].Name);
    }

    [Theory]
    [InlineData(
        "ScriptSampleTestStrings/GenericModel/JavascriptGenericModel_GenericModel.txt",
        "ScriptSampleTestStrings/GenericModel/JavascriptGenericModel_GenericModel2.txt",
        "ScriptSampleTestStrings/GenericModel/JavascriptGenericModel_NestedGenericModel.txt",
        "ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt"
    )]
    public async Task GenerateSampleCode_NestedGenericModel_ModelGivenAsObject_ShouldReturnGenericModel_Javascript(
        string pathToGenericModel,
        string pathToGenericModel2,
        string pathToNestedGenericModel,
        string pathToSampleCode
    )
    {
        var genericModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToGenericModel,
            TestContext.Current.CancellationToken
        );
        var genericModel2Content = await RelativeFileContentProvider.GetFileContentAsync(
            pathToGenericModel2,
            TestContext.Current.CancellationToken
        );
        var nestedGenericModelContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToNestedGenericModel,
            TestContext.Current.CancellationToken
        );
        var sampleCodeContent = await RelativeFileContentProvider.GetFileContentAsync(
            pathToSampleCode,
            TestContext.Current.CancellationToken
        );

        var sampleCode = await _sampleCodeGenerator.GetSampleCode(
            new List<object> { new NestedGenericModel() },
            TestContext.Current.CancellationToken
        );

        Assert.Equal(4, sampleCode.Count);

        Assert.Equal(genericModelContent, sampleCode[0].Content);
        Assert.Equal(genericModel2Content, sampleCode[1].Content);
        Assert.Equal(nestedGenericModelContent, sampleCode[2].Content);
        Assert.Equal(sampleCodeContent, sampleCode[3].Content);
    }
}
