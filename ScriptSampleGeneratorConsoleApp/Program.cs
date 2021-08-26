using System.IO;
using CommandLine;
using ScriptBee.Models.Dummy;
using ScriptBee.Scripts.ScriptSampleGenerators;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using ScriptSampleGeneratorConsoleApp.Exceptions;

namespace ScriptSampleGeneratorConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(options =>
            {
                SampleCodeProvider sampleCodeProvider = SampleCodeProvider.Instance;

                switch (options.ModelType)
                {
                    case "dummy":
                    {
                        switch (options.ScriptType)
                        {
                            case "python":
                            {
                                var generatedTemplate =
                                    new ScriptSampleGenerator(new PythonStrategyGenerator(sampleCodeProvider)).Generate(
                                        typeof(DummyModel));

                                WriteScript(options.OutputPath, generatedTemplate, "script.py");

                                break;
                            }
                            case "javascript":
                            {
                                var generatedTemplate =
                                    new ScriptSampleGenerator(new JavascriptStrategyGenerator(sampleCodeProvider))
                                        .Generate(
                                            typeof(DummyModel));

                                WriteScript(options.OutputPath, generatedTemplate, "script.js");

                                break;
                            }
                            case "csharp":
                            {
                                var generatedTemplate =
                                    new ScriptSampleGenerator(new CSharpStrategyGenerator(sampleCodeProvider)).Generate(
                                        typeof(DummyModel));

                                WriteScript(options.OutputPath, generatedTemplate, "script.cs");

                                break;
                            }
                            default:
                            {
                                throw new UnsupportedScriptTypeException(
                                    $"Insert a valid programming language. {options.ScriptType} is not supported");
                            }
                        }
                    }
                        break;

                    default:
                    {
                        throw new UnsupportedModelTypeException(
                            $"Insert a valid model type. {options.ModelType} is not supported");
                    }
                }
            });
        }

        private static void WriteScript(string outputPath, string generatedTemplate, string fileName)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                File.WriteAllText(fileName, generatedTemplate);
            }
            else
            {
                if (File.Exists(outputPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    File.WriteAllText(outputPath, generatedTemplate);
                }
                else
                {
                    var newPath = Path.Join(outputPath, fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    File.WriteAllText(newPath, generatedTemplate);
                }
            }
        }
    }
}