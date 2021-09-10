using System.Collections.Generic;
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
        public void Generate_WithPythonStrategy_MainModelGivenAsObject_ShouldReturnPythonSimpleModel()
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
            
            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("script",sampleCode[1].Name);
        }
        
        [Fact]
        public void Generate_WithJavascriptStrategy_MainModelGivenAsObject_ShouldReturnJavascriptSimpleModel()
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
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("script",sampleCode[1].Name);
        }

        [Fact]
        public void Generate_WithCsharpStrategy_DummyModelWithMethods_ShouldReturnCSharpSimpleModelWithMethods()
        {
            var modelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelWithMethods/CSharpDummyModel_WithMethods.txt");

            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt");
            
            _sampleCodeGenerator = new SampleCodeGenerator(new CSharpStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DummyModelWithMethods());
            
            Assert.Equal(2, sampleCode.Count);
            
            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);
            
            Assert.Equal("DummyModelWithMethods",sampleCode[0].Name);
            Assert.Equal("script",sampleCode[1].Name);
        }
        
        [Fact]
        public void Generate_WithPythonStrategy_DummyModelWithMethods_ShouldReturnPythonSimpleModel()
        {
            var modelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelWithMethods/PythonDummyModel_WithMethods.txt");

            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt");
            
            _sampleCodeGenerator = new SampleCodeGenerator(new PythonStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DummyModelWithMethods());
            
            Assert.Equal(2, sampleCode.Count);
            
            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);
            
            Assert.Equal("DummyModelWithMethods",sampleCode[0].Name);
            Assert.Equal("script",sampleCode[1].Name);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy_DummyModelWithMethods_ShouldReturnJavascriptSimpleModel()
        {
            var modelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/DummyModelWithMethods/JavascriptDummyModel_WithMethods.txt");

            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt");
            
            _sampleCodeGenerator = new SampleCodeGenerator(new JavascriptStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new DummyModelWithMethods());
            
            Assert.Equal(2, sampleCode.Count);
            
            Assert.Equal(modelContent, sampleCode[0].Content);
            Assert.Equal(sampleCodeContent, sampleCode[1].Content);
            
            Assert.Equal("DummyModelWithMethods",sampleCode[0].Name);
            Assert.Equal("script",sampleCode[1].Name);
        }

        [Fact]
        public void Generate_WithPythonStrategy_MainModelGivenAsObject_ShouldReturnPythonRecursiveModel()
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/PythonDummyModel.txt");
            var mainModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Python/PythonRecursiveModel_RecursiveModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Python_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new PythonStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new RecursiveModel());

            Assert.Equal(3, sampleCode.Count);
            
            Assert.Equal(dummyModelContent, sampleCode[0].Content);
            Assert.Equal(mainModelContent, sampleCode[1].Content);
            Assert.Equal(sampleCodeContent, sampleCode[2].Content);
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("script",sampleCode[2].Name);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy_MainModelGivenAsObject_ShouldReturnJavascriptRecursiveModel()
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt");
            var mainModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Javascript/JavascriptRecursiveModel_RecursiveModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new JavascriptStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new RecursiveModel());

            Assert.Equal(3, sampleCode.Count);
            
            Assert.Equal(dummyModelContent, sampleCode[0].Content);
            Assert.Equal(mainModelContent, sampleCode[1].Content);
            Assert.Equal(sampleCodeContent, sampleCode[2].Content);
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("script",sampleCode[2].Name);
        }
        
        [Fact]
        public void Generate_WithJavascriptStrategy_ModelsGivesAsListOfObjects_ShouldReturnJavascriptRecursiveModel()
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/JavascriptDummyModel.txt");
            var mainModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/Javascript/JavascriptRecursiveModel_RecursiveModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/Javascript_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new JavascriptStrategyGenerator(_fileContentProvider));

            List<object> models = new List<object>();
            models.Add(new DummyModel());
            models.Add(new RecursiveModel());
            
            var sampleCode = _sampleCodeGenerator.GetSampleCode(models);

            Assert.Equal(3, sampleCode.Count);
            
            Assert.Equal(dummyModelContent, sampleCode[0].Content);
            Assert.Equal(mainModelContent, sampleCode[1].Content);
            Assert.Equal(sampleCodeContent, sampleCode[2].Content);
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("script",sampleCode[2].Name);
        }

        [Fact]
        public void Generate_WithCSharpStrategy_MainModelGivenAsObject_ShouldReturnCSharpRecursiveModel()
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt");
            var mainModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/CSharp/CSharpRecursiveModel_RecursiveModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new CSharpStrategyGenerator(_fileContentProvider));
            var sampleCode = _sampleCodeGenerator.GetSampleCode(new RecursiveModel());

            Assert.Equal(3, sampleCode.Count);
            
            Assert.Equal(dummyModelContent, sampleCode[0].Content);
            Assert.Equal(mainModelContent, sampleCode[1].Content);
            Assert.Equal(sampleCodeContent, sampleCode[2].Content);
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("script",sampleCode[2].Name);
        }
        
        [Fact]
        public void Generate_WithCSharpStrategy_ModelsGivesAsListOfObjects_ShouldReturnCSharpRecursiveModel()
        {
            var dummyModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/CSharpDummyModel.txt");
            var mainModelContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/RecursiveModel/CSharp/CSharpRecursiveModel_RecursiveModel.txt");
            var sampleCodeContent =
                _fileContentProvider.GetFileContent(
                    "Scripts/ScriptSampleGenerators/ScriptSampleTestStrings/SampleCode/CSharp_SampleCode.txt");

            _sampleCodeGenerator = new SampleCodeGenerator(new CSharpStrategyGenerator(_fileContentProvider));

            List<object> models = new List<object>();
            models.Add(new DummyModel());
            models.Add(new RecursiveModel());
            
            var sampleCode = _sampleCodeGenerator.GetSampleCode(models);

            Assert.Equal(3, sampleCode.Count);
            
            Assert.Equal(dummyModelContent, sampleCode[0].Content);
            Assert.Equal(mainModelContent, sampleCode[1].Content);
            Assert.Equal(sampleCodeContent, sampleCode[2].Content);
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("script",sampleCode[2].Name);
        }
        
        [Fact]
        public void Generate_WithJavascriptStrategy_MainModelGivenAsObject_ShouldReturnJavascriptDeepModel()
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
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("RecursiveModel2",sampleCode[2].Name);
            Assert.Equal("EmptyModel",sampleCode[3].Name);
            Assert.Equal("DeepModel",sampleCode[4].Name);
            Assert.Equal("script",sampleCode[5].Name);
        }
        
        [Fact]
        public void Generate_WithJavascriptStrategy_ModelsGivenAsListOfObjects_ShouldReturnJavascriptDeepModel()
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
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("RecursiveModel2",sampleCode[2].Name);
            Assert.Equal("EmptyModel",sampleCode[3].Name);
            Assert.Equal("DeepModel",sampleCode[4].Name);
            Assert.Equal("script",sampleCode[5].Name);
        }

        [Fact]
        public void Generate_WithPythonStrategy_MainModelGivenAsObject_ShouldReturnPythonDeepModel()
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
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("RecursiveModel2",sampleCode[2].Name);
            Assert.Equal("EmptyModel",sampleCode[3].Name);
            Assert.Equal("DeepModel",sampleCode[4].Name);
            Assert.Equal("script",sampleCode[5].Name);
        }
        
        [Fact]
        public void Generate_WithPythonStrategy_ModelsGivenAsListOfObjects_ShouldReturnPythonDeepModel()
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
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("RecursiveModel2",sampleCode[2].Name);
            Assert.Equal("EmptyModel",sampleCode[3].Name);
            Assert.Equal("DeepModel",sampleCode[4].Name);
            Assert.Equal("script",sampleCode[5].Name);
        }

        [Fact]
        public void Generate_WithCSharpStrategy_MainModelGivenAsObject_ShouldReturnCSharpDeepModel()
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
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("RecursiveModel2",sampleCode[2].Name);
            Assert.Equal("EmptyModel",sampleCode[3].Name);
            Assert.Equal("DeepModel",sampleCode[4].Name);
            Assert.Equal("script",sampleCode[5].Name);
        }
        
        [Fact]
        public void Generate_WithCSharpStrategy_ModelsGivenAsListOfObjects_ShouldReturnCSharpDeepModel()
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
            
            Assert.Equal("DummyModel",sampleCode[0].Name);
            Assert.Equal("RecursiveModel",sampleCode[1].Name);
            Assert.Equal("RecursiveModel2",sampleCode[2].Name);
            Assert.Equal("EmptyModel",sampleCode[3].Name);
            Assert.Equal("DeepModel",sampleCode[4].Name);
            Assert.Equal("script",sampleCode[5].Name);
        }
    }
}