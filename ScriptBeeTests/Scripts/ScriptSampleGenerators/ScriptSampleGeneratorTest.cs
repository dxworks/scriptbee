using ScriptBee.Models.Dummy;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using Xunit;

namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class ScriptSampleGeneratorTest
    {
        private readonly FileContentProvider _fileContentProvider = new FileContentProvider();

        private readonly SampleCodeProvider _sampleCodeProvider = SampleCodeProvider.Instance;

            [Fact]
        public void Generate_WithPythonStrategy()
        {
            string expectedScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonSimpleModel.txt");

            string generatedScript =
                new ScriptSampleGenerator(new PythonStrategyGenerator(_sampleCodeProvider)).Generate(
                    typeof(DummyModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy()
        {
            string expectedScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptSimpleModel.txt");
            
            string generatedScript =
                new ScriptSampleGenerator(new JavascriptStrategyGenerator(_sampleCodeProvider)).Generate(
                    typeof(DummyModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithPythonStrategy_Recursive()
        {
            string expectedScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonRecursiveModel.txt");
            string generatedScript =
                new ScriptSampleGenerator(new PythonStrategyGenerator(_sampleCodeProvider)).Generate(
                    typeof(RecursiveModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy_Recursive()
        {
            string expectedScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptRecursiveModel.txt");
            
            string generatedScript =
                new ScriptSampleGenerator(new JavascriptStrategyGenerator(_sampleCodeProvider)).Generate(
                    typeof(RecursiveModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy_DeepModel()
        {
            string expectedScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDeepModelWithEmptyClass.txt");
            
            string generatedScript =
                new ScriptSampleGenerator(new JavascriptStrategyGenerator(_sampleCodeProvider)).Generate(
                    typeof(DeepModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithPythonStrategy_DeepModel()
        {
            string expectedScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDeepModelWithEmptyClass.txt");
            
            string generatedScript =
                new ScriptSampleGenerator(new PythonStrategyGenerator(_sampleCodeProvider)).Generate(
                    typeof(DeepModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithCSharpStrategy_RecursiveModel()
        {
            string expectedScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpRecursiveModel.txt");
            
            string generatedScript =
                new ScriptSampleGenerator(new CSharpStrategyGenerator(_sampleCodeProvider)).Generate(
                    typeof(RecursiveModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithCSharpStrategy_DeepModel()
        {
            string expectedScript =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDeepModelWithEmptyClass.txt");
            
            string generatedScript =
                new ScriptSampleGenerator(new CSharpStrategyGenerator(_sampleCodeProvider)).Generate(
                    typeof(DeepModel));

            Assert.Equal(expectedScript, generatedScript);
        }
    }
}