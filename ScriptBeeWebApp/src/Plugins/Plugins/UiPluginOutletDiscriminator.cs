using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Plugins;

public static class UiPluginOutletDiscriminator
{
    private static readonly Dictionary<string, Type> OutletTypes = new()
    {
        { "top-navigation-bar", typeof(TopNavigationBarOutlet) },
        { "side-panel", typeof(SidePanelOutlet) },
        { "file-previewer", typeof(FilePreviewerOutlet) },
    };

    public static Dictionary<string, Type> GetDiscriminatedTypes() => OutletTypes;
}
