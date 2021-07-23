using System.IO;
using System.Runtime.InteropServices;
using CommandLine;
using Microsoft.Scripting.Runtime;
using ScriptBee.Models;
using ScriptBee.Scripts;
using TemplateGeneratorConsoleApp.Exceptions;

namespace TemplateGeneratorConsoleApp
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
                                var generatedTemplate = new PythonTemplateGenerator().GenerateTemplate(new DummyModel());

                                if (string.IsNullOrWhiteSpace(options.OutputPath))
                                {
                                    File.WriteAllText("script.py", generatedTemplate);
                                }
                                else
                                {
                                    if (File.Exists(options.OutputPath))
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(options.OutputPath));
                                        File.WriteAllText(options.OutputPath, generatedTemplate);   
                                    }
                                    else
                                    {
                                        var newPath = Path.Join(options.OutputPath, "script.py");
                                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                                        File.WriteAllText(newPath, generatedTemplate); 
                                    }
                                }
                            }
                                break;
                            default:
                            {
                                throw new UnsupportedScriptTypeException($"Insert a valid programming language. {options.ScriptType} is not supported");
                            }
                        }
                    }
                        break;
                    
                    default:
                    {
                        throw new UnsupportedModelTypeException($"Insert a valid model type. {options.ModelType} is not supported");                    }
                }
                
                var modelType = options.ModelType;
                // options.ScriptType;
                // options.ScriptPath;
            });
        }
    }
}