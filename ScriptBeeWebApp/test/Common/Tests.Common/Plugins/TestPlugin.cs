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
                        new TopNavigationBarOutlet
                        {
                            Type = "top-navigation-bar",
                            ExposedModule = "./routes",
                            Path = "/my-plugin",
                            Label = "Flights",
                            Nested = true,
                            ComponentName = null,
                        },
                        new SidePanelOutlet
                        {
                            Type = "side-panel",
                            ExposedModule = "./Component",
                            Path = "/my-plugin",
                            Label = "My Plugin",
                            Nested = null,
                            ComponentName = null,
                            Icon = "favorite",
                        },
                        new FilePreviewerOutlet
                        {
                            Type = "file-previewer",
                            ExposedModule = "./Component",
                            Label = "My Plugin",
                            ComponentName = null,
                            Icon = null,
                            SupportedFileExtensions = ["json"],
                        },
                    },
                },
            ],
        }
    );
