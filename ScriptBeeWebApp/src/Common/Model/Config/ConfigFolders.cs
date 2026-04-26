namespace ScriptBee.Domain.Model.Config;

public static class ConfigFolders
{
    private static readonly string PathToUserFolder = Environment.GetFolderPath(
        Environment.SpecialFolder.UserProfile
    );

    private static readonly string PathToRoot = Path.Combine(PathToUserFolder, ".scriptbee");

    public static readonly string PathToPlugins = Path.Combine(PathToRoot, "plugins");

    public static readonly string PathToGatewayPlugins = Path.Combine(
        PathToRoot,
        "gateway",
        "plugins"
    );

    public static readonly string PathToProjects = Path.Combine(PathToRoot, "projects");
}
