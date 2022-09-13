using System;
using System.IO;

namespace ScriptBee.Config;

// todo cleanup unused code
public static class ConfigFolders
{
    private const string Root = ".scriptbee";

    private const string ModelsFolder = "models";

    private const string ResultsFolder = "results";

    private const string PluginsFolder = "plugins";

    private const string ProjectsFolder = "projects";

    public const string SrcFolder = "src";

    public const string GeneratedFolder = "generated";

    public static readonly string PathToUserFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public static readonly string PathToRoot = Path.Combine(PathToUserFolder, Root);

    public static readonly string PathToModels = Path.Combine(PathToRoot, ModelsFolder);

    public static readonly string PathToResults = Path.Combine(PathToRoot, ResultsFolder);

    public static readonly string PathToPlugins = Path.Combine(PathToRoot, PluginsFolder);

    public static readonly string PathToProjects = Path.Combine(PathToRoot, ProjectsFolder);
}
