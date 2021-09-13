using System;
using System.Collections.Generic;
using ScriptBee.PluginManager;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using Xunit;
using Moq;
using ScriptBeePlugin;

namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class SampleCodeGeneratorTest
    {
        private readonly RelativeFileContentProvider _fileContentProvider = new();
        private readonly Mock<ILoadersHolder> _loadersHolderMock = new();
        private SampleCodeGenerator _sampleCodeGenerator;

        public SampleCodeGeneratorTest()
        {
            _loadersHolderMock.Setup(holder => holder.GetAllLoaders()).Returns(new List<IModelLoader>
            {
                new TestModelLoader()
            });
        }

        [Theory]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt",
            typeof(CSharpStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt",
            typeof(PythonStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt",
            typeof(JavascriptStrategyGenerator))]
        public void GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDummyModel(
            string pathToModel, string pathToSampleCode,Type strategyGeneratorType)
        {
            var modelContent =
                _fileContentProvider.GetFileContent(pathToModel);

            var sampleCodeContent =
                _fileContentProvider.GetFileContent(pathToSampleCode);

            _sampleCodeGenerator = new SampleCodeGenerator((IStrategyGenerator)Activator.CreateInstance(strategyGeneratorType, new object[]{_fileContentProvider}), _loadersHolderMock.Object);
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DummyModel());

            Assert.Equal(2, sampleCode.Count);

            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);

            Assert.Equal("DummyModel", sampleCode[0].Name);
            Assert.Equal("script", sampleCode[1].Name);
        }

        [Theory]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/CSharp/CSharpRecursiveModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt",
            typeof(CSharpStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Python/PythonRecursiveModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt",
            typeof(PythonStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Javascript/JavascriptRecursiveModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt",
            typeof(JavascriptStrategyGenerator))]
        public void GenerateSampleCode_MainModelGivenAsObject_ShouldReturnRecursiveModel(
            string pathToDummyModel, string pathToMainModel, string pathToSampleCode,Type strategyGeneratorType)
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(pathToDummyModel);
            var mainModelContent =
                _fileContentProvider.GetFileContent(pathToMainModel);
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(pathToSampleCode);

            _sampleCodeGenerator = new SampleCodeGenerator((IStrategyGenerator)Activator.CreateInstance(strategyGeneratorType, new object[]{_fileContentProvider}), _loadersHolderMock.Object);
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new RecursiveModel());

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
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelInheritor/CSharp/CSharpDummyModelInheritor_DummyModelInheritor.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt",
            typeof(CSharpStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelInheritor/Python/PythonDummyModelInheritor_DummyModelInheritor.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt",
            typeof(PythonStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelInheritor/Javascript/JavascriptDummyModelInheritor_DummyModelInheritor.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt",
            typeof(JavascriptStrategyGenerator))]
        public void GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDummyModelInheritor(
            string pathToDummyModel, string pathToMainModel, string pathToSampleCode,Type strategyGeneratorType)
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(pathToDummyModel);
            var mainModelContent =
                _fileContentProvider.GetFileContent(pathToMainModel);
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(pathToSampleCode);

            _sampleCodeGenerator = new SampleCodeGenerator((IStrategyGenerator)Activator.CreateInstance(strategyGeneratorType, new object[]{_fileContentProvider}), _loadersHolderMock.Object);
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DummyModelInheritor());

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
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/CSharp/CSharpRecursiveModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt",
            typeof(CSharpStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Python/PythonRecursiveModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt",
            typeof(PythonStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Javascript/JavascriptRecursiveModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt",
            typeof(JavascriptStrategyGenerator))]
        public void GenerateSampleCode_ModelsGivenAsListOfObjects_ShouldReturnRecursiveModel(
            string pathToDummyModel, string pathToMainModel, string pathToSampleCode,Type strategyGeneratorType)
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(pathToDummyModel);
            var mainModelContent =
                _fileContentProvider.GetFileContent(pathToMainModel);
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(pathToSampleCode);

            _sampleCodeGenerator = new SampleCodeGenerator((IStrategyGenerator)Activator.CreateInstance(strategyGeneratorType, new object[]{_fileContentProvider}), _loadersHolderMock.Object);
            
            List<object> models = new List<object>();
            models.Add(new DummyModel());
            models.Add(new RecursiveModel());
            
            var sampleCode = _sampleCodeGenerator.GetSampleCode(models);

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
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_EmptyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_RecursiveModel2.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_DeepModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt",
            typeof(CSharpStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_EmptyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_RecursiveModel2.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_DeepModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt",
            typeof(PythonStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_EmptyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_RecursiveModel2.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_DeepModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt",
            typeof(JavascriptStrategyGenerator))]
        public void GenerateSampleCode_MainModelGivenAsObject_ShouldReturnDeepModel(
            string pathToDummyModel,string pathToEmptyModel, string pathToRecursiveModel, string pathToRecursiveModel2, string pathToMainModel, string pathToSampleCode,Type strategyGeneratorType)
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(pathToDummyModel);
            var emptyModelContent =
                _fileContentProvider.GetFileContent(pathToEmptyModel);
            var recursiveModelContent =
                _fileContentProvider.GetFileContent(pathToRecursiveModel);
            var recursiveModel2Content =
                _fileContentProvider.GetFileContent(pathToRecursiveModel2);
            var deepModelContent =
                _fileContentProvider.GetFileContent(pathToMainModel);
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(pathToSampleCode);

            _sampleCodeGenerator = new SampleCodeGenerator((IStrategyGenerator)Activator.CreateInstance(strategyGeneratorType, new object[]{_fileContentProvider}), _loadersHolderMock.Object);
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DeepModel());

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
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_EmptyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_RecursiveModel2.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_DeepModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt",
            typeof(CSharpStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_EmptyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_RecursiveModel2.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_DeepModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt",
            typeof(PythonStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_EmptyModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_RecursiveModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_RecursiveModel2.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_DeepModel.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt",
            typeof(JavascriptStrategyGenerator))]
        public void GenerateSampleCode_ModelsGivenAsListOfObjects_ShouldReturnDeepModel(
            string pathToDummyModel,string pathToEmptyModel, string pathToRecursiveModel, string pathToRecursiveModel2, string pathToMainModel, string pathToSampleCode,Type strategyGeneratorType)
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(pathToDummyModel);
            var emptyModelContent =
                _fileContentProvider.GetFileContent(pathToEmptyModel);
            var recursiveModelContent =
                _fileContentProvider.GetFileContent(pathToRecursiveModel);
            var recursiveModel2Content =
                _fileContentProvider.GetFileContent(pathToRecursiveModel2);
            var deepModelContent =
                _fileContentProvider.GetFileContent(pathToMainModel);
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(pathToSampleCode);

            _sampleCodeGenerator = new SampleCodeGenerator((IStrategyGenerator)Activator.CreateInstance(strategyGeneratorType, new object[]{_fileContentProvider}), _loadersHolderMock.Object);
            
            List<object> models = new List<object>();
            models.Add(new DummyModel());
            models.Add(new RecursiveModel());
            models.Add(new RecursiveModel2());
            models.Add(new EmptyModel());
            models.Add(new DeepModel());
            
            var sampleCode = _sampleCodeGenerator.GetSampleCode(models);

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
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelWithMethods/CSharpDummyModel_WithMethods.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt",
            typeof(CSharpStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelWithMethods/PythonDummyModel_WithMethods.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt",
            typeof(PythonStrategyGenerator))]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelWithMethods/JavascriptDummyModel_WithMethods.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt",
            typeof(JavascriptStrategyGenerator))]
        public void GenerateSampleCode_DummyModelWithMethods_ShouldReturnDummyModelWithMethods(
            string pathToModel, string pathToSampleCode, Type strategyGeneratorType)
        {
            var modelContent =
                _fileContentProvider.GetFileContent(pathToModel);

            var sampleCodeContent =
                _fileContentProvider.GetFileContent(pathToSampleCode);

            _sampleCodeGenerator = new SampleCodeGenerator((IStrategyGenerator)Activator.CreateInstance(strategyGeneratorType, new object[]{_fileContentProvider}), _loadersHolderMock.Object);
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DummyModelWithMethods());

            Assert.Equal(2, sampleCode.Count);

            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);

            Assert.Equal("DummyModelWithMethods", sampleCode[0].Name);
            Assert.Equal("script", sampleCode[1].Name);
        }
        
        [Theory]
        [InlineData(
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelWithMethods/CSharpDummyModel_WithMethods_Expando.txt",
            "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt",
            typeof(DummyModelWithMethodsExpando))]
        public void GenerateSampleCode_WithCSharpStrategy_DummyModelWithMethodsExpando_ShouldReturnCSharpSimpleModelWithMethods(
            string pathToModel, string pathToSampleCode, Type type)
        {
            var modelContent =
                _fileContentProvider.GetFileContent(pathToModel);

            var sampleCodeContent =
                _fileContentProvider.GetFileContent(pathToSampleCode);

            _sampleCodeGenerator = new SampleCodeGenerator(new CSharpStrategyGenerator(_fileContentProvider), _loadersHolderMock.Object);
            var sampleCode = _sampleCodeGenerator.GetSampleCode(Activator.CreateInstance(type));

            Assert.Equal(2, sampleCode.Count);

            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);

            Assert.Equal(type.Name, sampleCode[0].Name);
            Assert.Equal("script", sampleCode[1].Name);
        }
    }
}