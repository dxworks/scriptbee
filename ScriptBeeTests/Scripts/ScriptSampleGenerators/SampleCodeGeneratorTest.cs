using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using Xunit;

namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class SampleCodeGeneratorTest
    {
        private readonly RelativeFileContentProvider _fileContentProvider = new();
        private SampleCodeGenerator _sampleCodeGenerator;
        
        [Fact]
        public void Generate_ShouldReturnPythonSimpleModel()
        {
            var modelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt");

            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt");
            
            _sampleCodeGenerator = new SampleCodeGenerator(new PythonStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DummyModel());
            
            Assert.Equal(2, sampleCode.Count);
            // todo test sampleCode[0].Name
            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy()
        {
            var modelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt");
            
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new JavascriptStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DummyModel());

            Assert.Equal(2, sampleCode.Count);
            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);
        }

        [Fact]
        public void Generate_WithPythonStrategy_Recursive()
        {
            var recursiveModelDummyContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt");
            var mainModelScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Python/PythonRecursiveModel_RecursiveModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new PythonStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new RecursiveModel());

            Assert.Equal(3, sampleCode.Count);
            Assert.Equal(recursiveModelDummyContent, sampleCode[0].Content);
            Assert.Equal(mainModelScript, sampleCode[1].Content);
            Assert.Equal(sampleCodeContent, sampleCode[2].Content);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy_Recursive()
        {
            var recursiveModelDummyContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt");
            var mainModelScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Javascript/JavascriptRecursiveModel_RecursiveModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new JavascriptStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new RecursiveModel());

            Assert.Equal(3, sampleCode.Count);
            Assert.Equal(recursiveModelDummyContent, sampleCode[0].Content);
            Assert.Equal(mainModelScript, sampleCode[1].Content);
            Assert.Equal(sampleCodeContent, sampleCode[2].Content);
        }

        [Fact]
        public void Generate_WithCSharpStrategy_RecursiveModel()
        {
            var recursiveModelDummyContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt");
            var mainModelScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/CSharp/CSharpRecursiveModel_RecursiveModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new CSharpStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new RecursiveModel());

            Assert.Equal(3, sampleCode.Count);
            Assert.Equal(recursiveModelDummyContent, sampleCode[0].Content);
            Assert.Equal(mainModelScript, sampleCode[1].Content);
            Assert.Equal(sampleCodeContent, sampleCode[2].Content);
        }
        
        [Fact]
        public void Generate_WithJavascriptStrategy_DeepModel()
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt");
            var emptyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_EmptyModel.txt");
            var recursiveModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_RecursiveModel.txt");
            var recursiveModel2Content =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_RecursiveModel2.txt");
            var deepModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Javascript/JavascriptDeepModel_DeepModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt");
            
            _sampleCodeGenerator = new SampleCodeGenerator(new JavascriptStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DeepModel());

            Assert.Equal(6, sampleCode.Count);
            Assert.Equal(dummyModelContent, sampleCode[0].Content);
            Assert.Equal(recursiveModelContent, sampleCode[1].Content);
            Assert.Equal(recursiveModel2Content, sampleCode[2].Content);
            Assert.Equal(emptyModelContent, sampleCode[3].Content);
            Assert.Equal(deepModelContent, sampleCode[4].Content);
            Assert.Equal(sampleCodeContent, sampleCode[5].Content);
        }

        [Fact]
        public void Generate_WithPythonStrategy_DeepModel()
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt");
            var emptyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_EmptyModel.txt");
            var recursiveModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_RecursiveModel.txt");
            var recursiveModel2Content =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_RecursiveModel2.txt");
            var deepModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/Python/PythonDeepModel_DeepModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt");
            
            _sampleCodeGenerator = new SampleCodeGenerator(new PythonStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DeepModel());

            Assert.Equal(6, sampleCode.Count);
            Assert.Equal(dummyModelContent, sampleCode[0].Content);
            Assert.Equal(recursiveModelContent, sampleCode[1].Content);
            Assert.Equal(recursiveModel2Content, sampleCode[2].Content);
            Assert.Equal(emptyModelContent, sampleCode[3].Content);
            Assert.Equal(deepModelContent, sampleCode[4].Content);
            Assert.Equal(sampleCodeContent, sampleCode[5].Content);
        }

        [Fact]
        public void Generate_WithCSharpStrategy_DeepModel()
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt");
            var emptyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_EmptyModel.txt");
            var recursiveModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_RecursiveModel.txt");
            var recursiveModel2Content =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_RecursiveModel2.txt");
            var deepModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DeepModel/CSharp/CSharpDeepModel_DeepModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt");
            
            _sampleCodeGenerator = new SampleCodeGenerator(new CSharpStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DeepModel());

            Assert.Equal(6, sampleCode.Count);
            Assert.Equal(dummyModelContent, sampleCode[0].Content);
            Assert.Equal(recursiveModelContent, sampleCode[1].Content);
            Assert.Equal(recursiveModel2Content, sampleCode[2].Content);
            Assert.Equal(emptyModelContent, sampleCode[3].Content);
            Assert.Equal(deepModelContent, sampleCode[4].Content);
            Assert.Equal(sampleCodeContent, sampleCode[5].Content);
        }
    }
}