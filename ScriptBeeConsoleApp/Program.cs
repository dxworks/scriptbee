using CommandLine;
using ScriptBee.Models.Dummy;
using ScriptBee.Scripts;
using ScriptBee.Utils.ValidScriptExtractors;
using ScriptBeeConsoleApp.Exceptions;

namespace ScriptBeeConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(options =>
            {
                var fileLoader = new FileLoader();
                var scriptContent = fileLoader.LoadFileContent(options.ScriptPath);

                switch (options.ModelType)
                {
                    case "dummy":
                    {
                        DummyModelLoader dummyModelLoader = new DummyModelLoader();
                        var dummyModel = dummyModelLoader.LoadModel(fileLoader.LoadFileContent(options.ModelPath));

                        switch (options.ScriptType)
                        {
                            case "python":
                            {
                                PythonDummyScriptRunner pythonDummyScriptRunner =
                                    new PythonDummyScriptRunner(new PythonValidScriptExtractor());
                                pythonDummyScriptRunner.RunScript(dummyModel, scriptContent);
                            }
                                break;
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
    }
}