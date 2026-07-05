using System.ComponentModel;
using System.Text.Json.Serialization;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[Description("Base class for plugin extension points")]
[JsonDerivedType(typeof(WebNestedPluginExtensionPoint), typeDiscriminator: PluginKind.Plugin)]
[JsonDerivedType(typeof(WebLoaderPluginExtensionPoint), typeDiscriminator: PluginKind.Loader)]
[JsonDerivedType(typeof(WebLinkerPluginExtensionPoint), typeDiscriminator: PluginKind.Linker)]
[JsonDerivedType(
    typeof(WebScriptGeneratorPluginExtensionPoint),
    typeDiscriminator: PluginKind.ScriptGenerator
)]
[JsonDerivedType(
    typeof(WebScriptRunnerPluginExtensionPoint),
    typeDiscriminator: PluginKind.ScriptRunner
)]
[JsonDerivedType(
    typeof(WebHelperFunctionsPluginExtensionPoint),
    typeDiscriminator: PluginKind.HelperFunctions
)]
[JsonDerivedType(typeof(WebUiPluginExtensionPoint), typeDiscriminator: PluginKind.Ui)]
public record WebPluginExtensionPoint(
    [property: JsonIgnore] string Kind,
    string EntryPoint,
    string Version
);

[Description("Represents a plugin from a bundle.")]
public record WebNestedPluginExtensionPoint(string Kind, string EntryPoint, string Version)
    : WebPluginExtensionPoint(Kind, EntryPoint, Version);

[Description("Represents a loader plugin extension point.")]
public record WebLoaderPluginExtensionPoint(string Kind, string EntryPoint, string Version)
    : WebPluginExtensionPoint(Kind, EntryPoint, Version);

[Description("Represents a linker plugin extension point.")]
public record WebLinkerPluginExtensionPoint(string Kind, string EntryPoint, string Version)
    : WebPluginExtensionPoint(Kind, EntryPoint, Version);

[Description("Represents a script generator plugin extension point.")]
public record WebScriptGeneratorPluginExtensionPoint(
    string Kind,
    string EntryPoint,
    string Version,
    string Language,
    string Extension
) : WebPluginExtensionPoint(Kind, EntryPoint, Version);

[Description("Represents a script runner plugin extension point.")]
public record WebScriptRunnerPluginExtensionPoint(
    string Kind,
    string EntryPoint,
    string Version,
    string Language,
    string Extension
) : WebPluginExtensionPoint(Kind, EntryPoint, Version);

[Description("Represents a helper functions plugin extension point.")]
public record WebHelperFunctionsPluginExtensionPoint(string Kind, string EntryPoint, string Version)
    : WebPluginExtensionPoint(Kind, EntryPoint, Version);

[Description("Represents a UI plugin extension point.")]
public record WebUiPluginExtensionPoint(
    string Kind,
    string EntryPoint,
    string Version,
    string RemoteName,
    string RemoteEntry,
    IEnumerable<WebInstalledPluginExtensionPointOutletBase> Outlets
) : WebPluginExtensionPoint(Kind, EntryPoint, Version);
