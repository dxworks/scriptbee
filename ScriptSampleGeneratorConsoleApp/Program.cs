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
                switch (options.ModelType)
                {
                    case "dummy":
                    {
                        switch (options.ScriptType)
                        {
                            case "python":
                            {
                                var generatedTemplate =
                                    new ScriptSampleGenerator(new PythonStrategyGenerator(new SampleCodeProvider())).Generate(
                                        typeof(DummyModel));

                                WriteScript(options.OutputPath, generatedTemplate, "script.py");

                                break;
                            }
                            case "javascript":
                            {
                                var generatedTemplate =
                                    new ScriptSampleGenerator(new JavascriptStrategyGenerator(new SampleCodeProvider())).Generate(
                                        typeof(DummyModel));

                                WriteScript(options.OutputPath, generatedTemplate, "script.js");

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