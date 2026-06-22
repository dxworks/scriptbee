using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Rest.Contracts;

namespace ScriptBee.Rest.Tests;

public class RestPluginExtensionPointTests
{
    [Fact]
    public void Map_Should_Map_TopNavigationBarOutlet()
    {
        var rest = new RestPluginExtensionPoint
        {
            Kind = PluginKind.Ui,
            EntryPoint = string.Empty,
            Version = string.Empty,
            RemoteName = "name",
            RemoteEntry = "entry",
            Outlets = new List<RestUiPluginExtensionPointOutlet>
            {
                new RestUiPluginExtensionPointOutlet
                {
                    Type = "top-navigation-bar",
                    ExposedModule = "./routes",
                    Path = "/my-plugin",
                    Label = "Flights",
                    Nested = true,
                    ComponentName = null,
                },
            },
        };

        var mapped = rest.Map();

        var ui = mapped as UiPluginExtensionPoint;
        ui.ShouldNotBeNull();
        var outlet = ui.Outlets.Single();
        outlet.ShouldBeOfType<TopNavigationBarOutlet>();
        var top = (TopNavigationBarOutlet)outlet;
        top.ExposedModule.ShouldBe("./routes");
        top.Path.ShouldBe("/my-plugin");
        top.Label.ShouldBe("Flights");
        top.Nested.ShouldBe(true);
    }

    [Fact]
    public void Map_Should_Map_SidePanelOutlet()
    {
        var rest = new RestPluginExtensionPoint
        {
            Kind = PluginKind.Ui,
            EntryPoint = string.Empty,
            Version = string.Empty,
            RemoteName = "name",
            RemoteEntry = "entry",
            Outlets = new List<RestUiPluginExtensionPointOutlet>
            {
                new RestUiPluginExtensionPointOutlet
                {
                    Type = "side-panel",
                    ExposedModule = "./Component",
                    Path = "/my-plugin",
                    Label = "My Plugin",
                    Nested = null,
                    ComponentName = null,
                    Icon = "favorite",
                },
            },
        };

        var mapped = rest.Map();

        var ui = mapped as UiPluginExtensionPoint;
        ui.ShouldNotBeNull();
        var outlet = ui.Outlets.Single();
        outlet.ShouldBeOfType<SidePanelOutlet>();
        var side = (SidePanelOutlet)outlet;
        side.Icon.ShouldBe("favorite");
    }

    [Fact]
    public void Map_Should_Map_FilePreviewerOutlet()
    {
        var rest = new RestPluginExtensionPoint
        {
            Kind = PluginKind.Ui,
            EntryPoint = string.Empty,
            Version = string.Empty,
            RemoteName = "name",
            RemoteEntry = "entry",
            Outlets = new List<RestUiPluginExtensionPointOutlet>
            {
                new()
                {
                    Type = "file-previewer",
                    ExposedModule = "./Component",
                    Label = "My Plugin",
                    ComponentName = null,
                    Icon = null,
                    SupportedFileExtensions = ["json"],
                },
            },
        };

        var mapped = rest.Map();

        var ui = mapped as UiPluginExtensionPoint;
        ui.ShouldNotBeNull();
        var outlet = ui.Outlets.Single();
        outlet.ShouldBeOfType<FilePreviewerOutlet>();
        var file = (FilePreviewerOutlet)outlet;
        file.SupportedFileExtensions.ShouldNotBeNull();
        file.SupportedFileExtensions.ShouldContain("json");
    }
}
