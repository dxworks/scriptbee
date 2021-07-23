using CommandLine;

namespace TemplateGeneratorConsoleApp
{
    public class CommandLineOptions
    {
        [Value(0, Required = true, MetaName = "Script Type",
            HelpText = "Supported Script types: csharp, python, javascript")]
        public string ScriptType { get; set; }

        [Value(1, Required = true, MetaName = "Model Type",
            HelpText = "Supported Model types: dummy")]
        public string ModelType { get; set; }

        [Value(2, Required = false, MetaName = "Output Path")]

        public string OutputPath { get; set; }
    }
}