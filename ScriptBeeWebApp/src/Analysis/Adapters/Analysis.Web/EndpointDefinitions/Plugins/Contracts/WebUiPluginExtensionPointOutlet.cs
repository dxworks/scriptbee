using System.ComponentModel;
using System.Text.Json.Serialization;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(
    typeof(WebInstalledPluginTopNavigationBarOutlet),
    typeDiscriminator: OutletTypes.TopNavigationBar
)]
[JsonDerivedType(
    typeof(WebInstalledPluginSidePanelOutlet),
    typeDiscriminator: OutletTypes.SidePanel
)]
[JsonDerivedType(
    typeof(WebInstalledPluginFilePreviewerOutlet),
    typeDiscriminator: OutletTypes.FilePreviewer
)]
[Description("Base class for outlets of a plugin extension point.")]
public abstract record WebInstalledPluginExtensionPointOutletBase(
    [property: JsonIgnore] string Type
)
{
    public static WebInstalledPluginExtensionPointOutletBase Map(
        UiPluginExtensionPointOutlet outlet
    ) =>
        outlet switch
        {
            TopNavigationBarOutlet topNavigationBarOutlet =>
                new WebInstalledPluginTopNavigationBarOutlet(
                    topNavigationBarOutlet.Type,
                    topNavigationBarOutlet.ExposedModule,
                    topNavigationBarOutlet.Label,
                    topNavigationBarOutlet.Path,
                    topNavigationBarOutlet.Nested,
                    topNavigationBarOutlet.ComponentName
                ),
            SidePanelOutlet sidePanelOutlet => new WebInstalledPluginSidePanelOutlet(
                sidePanelOutlet.Type,
                sidePanelOutlet.ExposedModule,
                sidePanelOutlet.Label,
                sidePanelOutlet.Path,
                sidePanelOutlet.Nested,
                sidePanelOutlet.ComponentName,
                sidePanelOutlet.Icon
            ),
            FilePreviewerOutlet filePreviewerOutlet => new WebInstalledPluginFilePreviewerOutlet(
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
public record WebInstalledPluginTopNavigationBarOutlet(
    string Type,
    string ExposedModule,
    string Label,
    string Path,
    bool? Nested,
    string? ComponentName
) : WebInstalledPluginExtensionPointOutletBase(Type);

[Description("Represents an element from the Side Hamburger Panel with route.")]
public record WebInstalledPluginSidePanelOutlet(
    string Type,
    string ExposedModule,
    string Label,
    string Path,
    bool? Nested,
    string? ComponentName,
    string Icon
) : WebInstalledPluginExtensionPointOutletBase(Type);

[Description("Represents a component for file previewer of output results.")]
public record WebInstalledPluginFilePreviewerOutlet(
    string Type,
    string ExposedModule,
    string Label,
    string? ComponentName,
    string? Icon,
    List<string>? SupportedFileExtensions
) : WebInstalledPluginExtensionPointOutletBase(Type);
