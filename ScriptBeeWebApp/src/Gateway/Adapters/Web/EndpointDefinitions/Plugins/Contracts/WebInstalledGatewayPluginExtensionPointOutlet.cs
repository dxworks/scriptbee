using System.ComponentModel;
using System.Text.Json.Serialization;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(
    typeof(WebInstalledGatewayPluginTopNavigationBarOutlet),
    typeDiscriminator: "top-navigation-bar"
)]
[JsonDerivedType(typeof(WebInstalledGatewayPluginSidePanelOutlet), typeDiscriminator: "side-panel")]
[JsonDerivedType(
    typeof(WebInstalledGatewayPluginFilePreviewerOutlet),
    typeDiscriminator: "file-previewer"
)]
[Description("Base class for outlets of an installed gateway plugin extension point.")]
public abstract record WebInstalledGatewayPluginExtensionPointOutletBase(
    [property: JsonIgnore] string Type
);

[Description("Represents an outlet of an installed gateway plugin extension point.")]
public record WebInstalledGatewayPluginExtensionPointOutlet(string Type)
    : WebInstalledGatewayPluginExtensionPointOutletBase(Type)
{
    public static WebInstalledGatewayPluginExtensionPointOutletBase Map(
        UiPluginExtensionPointOutlet outlet
    ) =>
        outlet switch
        {
            TopNavigationBarOutlet topNavigationBarOutlet =>
                new WebInstalledGatewayPluginTopNavigationBarOutlet(
                    topNavigationBarOutlet.Type,
                    topNavigationBarOutlet.ExposedModule,
                    topNavigationBarOutlet.Label,
                    topNavigationBarOutlet.Path,
                    topNavigationBarOutlet.Nested,
                    topNavigationBarOutlet.ComponentName
                ),
            SidePanelOutlet sidePanelOutlet => new WebInstalledGatewayPluginSidePanelOutlet(
                sidePanelOutlet.Type,
                sidePanelOutlet.ExposedModule,
                sidePanelOutlet.Label,
                sidePanelOutlet.Path,
                sidePanelOutlet.Nested,
                sidePanelOutlet.ComponentName,
                sidePanelOutlet.Icon
            ),
            FilePreviewerOutlet filePreviewerOutlet =>
                new WebInstalledGatewayPluginFilePreviewerOutlet(
                    filePreviewerOutlet.Type,
                    filePreviewerOutlet.ExposedModule,
                    filePreviewerOutlet.Label,
                    filePreviewerOutlet.ComponentName,
                    filePreviewerOutlet.Icon,
                    filePreviewerOutlet.SupportedFileExtensions
                ),
            _ => throw new ArgumentOutOfRangeException(nameof(outlet), outlet, null),
        };
}

[Description("Represents an element from the Top Navigation Bar with route.")]
public record WebInstalledGatewayPluginTopNavigationBarOutlet(
    string Type,
    string ExposedModule,
    string Label,
    string Path,
    bool? Nested,
    string? ComponentName
) : WebInstalledGatewayPluginExtensionPointOutletBase(Type);

[Description("Represents an element from the Side Hamburger Panel with route.")]
public record WebInstalledGatewayPluginSidePanelOutlet(
    string Type,
    string ExposedModule,
    string Label,
    string Path,
    bool? Nested,
    string? ComponentName,
    string Icon
) : WebInstalledGatewayPluginExtensionPointOutletBase(Type);

[Description("Represents a component for file previewer of output results.")]
public record WebInstalledGatewayPluginFilePreviewerOutlet(
    string Type,
    string ExposedModule,
    string Label,
    string? ComponentName,
    string? Icon,
    List<string>? SupportedFileExtensions
) : WebInstalledGatewayPluginExtensionPointOutletBase(Type);
