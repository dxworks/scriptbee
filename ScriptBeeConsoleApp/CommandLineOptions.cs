using CommandLine;

namespace ScriptBeeConsoleApp
{
    public class CommandLineOptions
    {
        [Value(0, Required = true, MetaName = "Model Type",
            HelpText = "Supported Model types: dummy")]
        public string ModelType { get; set; }

        [Value(1, Required = true, MetaName = "Model Path",
            HelpText = "Location where model resides")]
        public string ModelPath { get; set; }

        [Value(2, Required = true, MetaName = "Script Type",
            HelpText = "Supported Script types: csharp, python, javascript")]
        public string ScriptType { get; set; }

        [Value(3, Required = true, MetaName = "Script Path",
            HelpText = "Location where script resides")]
        public string ScriptPath { get; set; }
    }
}