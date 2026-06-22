using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Tests.Common.Plugins;

public record TestPlugin(PluginId Id, string FolderPath = "path")
    : Plugin(
        FolderPath,
        Id,
        new TestPluginManifest { ExtensionPoints = [new TestPluginExtensionPoint()] }
    );

public class TestPluginManifest : PluginManifest;

public class TestPluginExtensionPoint : PluginExtensionPoint;

public record TestUiPlugin(PluginId Id, string FolderPath = "path")
    : Plugin(
        FolderPath,
        Id,
        new TestPluginManifest
        {
            ExtensionPoints =
            [
                new UiPluginExtensionPoint
                {
                    Kind = "UI",
                    RemoteName = "scriptbee-ui-plugin-example",
                    RemoteEntry = "http://localhost:4201/remoteEntry.json",
                    Outlets = new List<UiPluginExtensionPointOutlet>
                    {
                        new TopNavigationBarOutlet(
                            "top-navigation-bar",
                            "./routes",
                            "/my-plugin",
                            "Flights",
                            true,
                            null
                        ),
                        new SidePanelOutlet(
                            "side-panel",
                            "./Component",
                            "/my-plugin",
                            "My Plugin",
                            null,
                            null,
                            "favorite"
                        ),
                        new FilePreviewerOutlet(
                            "file-previewer",
                            "./Component",
                            "My Plugin",
                            null,
                            null,
                            ["json"]
                        ),
                    },
                },
            ],
        }
    );
