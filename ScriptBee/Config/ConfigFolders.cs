using System;
using System.IO;

namespace ScriptBee.Config
{
    public static class ConfigFolders
    {
        private const string Root = ".scriptbee";

        private const string ModelsFolder = "models";

        private const string ResultsFolder = "results";

        private const string PluginsFolder = "plugins";
        
        public static readonly string PathToRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Root);

        public static readonly string PathToModels = Path.Combine(PathToRoot, ModelsFolder);

        public static readonly string PathToResults = Path.Combine(PathToRoot, ResultsFolder);

        public static readonly string PathToPlugins = Path.Combine(PathToRoot, PluginsFolder);
    }
}