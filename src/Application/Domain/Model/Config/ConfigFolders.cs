namespace ScriptBee.Domain.Model.Config;

public static class ConfigFolders
{
    private const string Root = ".scriptbee";

    private const string PluginsFolder = "plugins";

    private const string ProjectsFolder = "projects";

    public const string SrcFolder = "src";

    public const string GeneratedFolder = "generated";

    private static readonly string PathToUserFolder = Environment.GetFolderPath(
        Environment.SpecialFolder.UserProfile
    );

    public static readonly string PathToRoot = Path.Combine(PathToUserFolder, Root);

    public static readonly string PathToPlugins = Path.Combine(PathToRoot, PluginsFolder);

    public static readonly string PathToProjects = Path.Combine(PathToRoot, ProjectsFolder);
}
