using CommandLine;
using DummyPlugin;
using HelperFunctions;
using ScriptBee.Plugins;
using ScriptBee.Scripts;
using ScriptBee.Scripts.ScriptRunners;
using ScriptRunnerConsoleApp.Exceptions;

namespace ScriptRunnerConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(options =>
            {
                var fileLoader = new FileLoader();
                var scriptContent = fileLoader.LoadFileContent(options.ScriptPath);
                var helperFunctionsMapper = new HelperFunctionsMapper();

                switch (options.ModelType)
                {
                    case "dummy":
                    {
                        DummyModelLoader dummyModelLoader = new DummyModelLoader();
                        var dummyModel = dummyModelLoader.LoadModel(fileLoader.LoadFileContent(options.ModelPath));

                        DummyScriptRunner dummyScriptRunner;
                        switch (options.ScriptType)
                        {
                            case "python":
                            {
                                dummyScriptRunner = new PythonDummyScriptRunner(helperFunctionsMapper);

                                break;
                            }
                            case "javascript":
                            {
                                dummyScriptRunner = new JavascriptDummyScriptRunner(helperFunctionsMapper);

                                break;
                            }
                            case "csharp":
                            {
                                dummyScriptRunner = new CSharpDummyScriptRunner(new PluginLoader("Plugins"));

                                break;
                            }
                            default:
                            {
                                throw new UnsupportedScriptTypeException(
                                    $"Insert a valid programming language. {options.ScriptType} is not supported");
                            }
                        }

                        dummyScriptRunner.RunScript(dummyModel, scriptContent);
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