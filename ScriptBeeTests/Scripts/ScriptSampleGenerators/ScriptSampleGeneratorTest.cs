using System;
using ScriptBee.Models.Dummy;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using Xunit;

namespace ScriptBeeTests.Scripts.ScriptSampleGenerators
{
    public class ScriptSampleGeneratorTest
    {
        [Fact]
        public void Generate_WithPythonStrategy()
        {
            const string expectedScript = @"class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool

model: DummyModel

# start script

print(model)

# end script
";
            string generatedScript =
                new ScriptSampleGenerator(new PythonStrategyGenerator(new SampleCodeProvider())).Generate(typeof(DummyModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy()
        {
            const string expectedScript = @"class DummyModel
{
    DummyNumber = 0;
    DummyString = '';
    IsDummy = true;
}

let model = new DummyModel();

// start script

print(model);

// end script
";
            string generatedScript =
                new ScriptSampleGenerator(new JavascriptStrategyGenerator(new SampleCodeProvider())).Generate(typeof(DummyModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithPythonStrategy_Recursive()
        {
            const string expectedScript = @"class RecursiveModel:
    longField: long
    dummyField1: DummyModel
    recursiveModel: RecursiveModel
    dummyField2: DummyModel

class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool

model: RecursiveModel

# start script

print(model)

# end script
";
            string generatedScript =
                new ScriptSampleGenerator(new PythonStrategyGenerator(new SampleCodeProvider())).Generate(typeof(RecursiveModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy_Recursive()
        {
            const string expectedScript = @"class RecursiveModel
{
    longField = 0;
    dummyField1 = new DummyModel();
    recursiveModel = new RecursiveModel();
    dummyField2 = new DummyModel();
}

class DummyModel
{
    DummyNumber = 0;
    DummyString = '';
    IsDummy = true;
}

let model = new RecursiveModel();

// start script

print(model);

// end script
";
            string generatedScript =
                new ScriptSampleGenerator(new JavascriptStrategyGenerator(new SampleCodeProvider())).Generate(typeof(RecursiveModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithJavascriptStrategy_DeepModel()
        {
            const string expectedScript = @"class DeepModel
{
    floatField = 0;
    recursiveModel1 = new RecursiveModel();
    recursiveModel2 = new RecursiveModel2();
    empty = new EmptyClass();
}

class RecursiveModel
{
    longField = 0;
    dummyField1 = new DummyModel();
    recursiveModel = new RecursiveModel();
    dummyField2 = new DummyModel();
}

class DummyModel
{
    DummyNumber = 0;
    DummyString = '';
    IsDummy = true;
}

class RecursiveModel2
{
    dummyField1 = new DummyModel();
    dummyField2 = new DummyModel();
    value = '';
}

class EmptyClass
{

}

let model = new DeepModel();

// start script

print(model);

// end script
";
            string generatedScript =
                new ScriptSampleGenerator(new JavascriptStrategyGenerator(new SampleCodeProvider())).Generate(typeof(DeepModel));

            Assert.Equal(expectedScript, generatedScript);
        }

        [Fact]
        public void Generate_WithPythonStrategy_DeepModel()
        {
            const string expectedScript = @"class DeepModel:
    floatField: float
    recursiveModel1: RecursiveModel
    recursiveModel2: RecursiveModel2
    empty: EmptyClass

class RecursiveModel:
    longField: long
    dummyField1: DummyModel
    recursiveModel: RecursiveModel
    dummyField2: DummyModel

class DummyModel:
    DummyNumber: int
    DummyString: str
    IsDummy: bool

class RecursiveModel2:
    dummyField1: DummyModel
    dummyField2: DummyModel
    value: str

class EmptyClass:
    pass

model: DeepModel

# start script

print(model)

# end script
";
            string generatedScript =
                new ScriptSampleGenerator(new PythonStrategyGenerator(new SampleCodeProvider())).Generate(typeof(DeepModel));

            Assert.Equal(expectedScript, generatedScript);
        }
        
        [Fact]
        public void Generate_WithCSharpStrategy_DeepModel()
        {
            const string expectedScript = @"using System;
public class DeepModel
{
    public float floatField;
    public RecursiveModel recursiveModel1;
    public RecursiveModel2 recursiveModel2;
    public EmptyClass empty;
}

public class RecursiveModel
{
    public long longField;
    public DummyModel dummyField1;
    public RecursiveModel recursiveModel;
    public DummyModel dummyField2;
}

public class DummyModel
{
    public int DummyNumber;
    public string DummyString;
    public boolean IsDummy;
}

public class RecursiveModel2
{
    public DummyModel dummyField1;
    public DummyModel dummyField2;
    public char value;
}

public class EmptyClass
{

}

public class ScriptContent
{
    public void ExecuteScript(DeepModel model)
    {
        // start script

        Console.WriteLine(model);

        // end script
    }
}
";
            string generatedScript =
                new ScriptSampleGenerator(new CSharpStrategyGenerator(new SampleCodeProvider())).Generate(typeof(DeepModel));

            Assert.Equal(expectedScript, generatedScript);
        }
    }
}