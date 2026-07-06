namespace ScriptBee.Domain.Model.Plugins.Manifest;

public static class PluginKind
{
    public const string Plugin = "Plugin";
    public const string Linker = "Linker";
    public const string Loader = "Loader";
    public const string ScriptGenerator = "ScriptGenerator";
    public const string ScriptRunner = "ScriptRunner";
    public const string HelperFunctions = "HelperFunctions";
    public const string Ui = "UI";
}

public static class OutletTypes
{
    public const string TopNavigationBar = "top-navigation-bar";
    public const string SidePanel = "side-panel";
    public const string FilePreviewer = "file-previewer";
}
