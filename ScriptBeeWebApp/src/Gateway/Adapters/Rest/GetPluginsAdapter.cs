using System.Text.Json;
using Refit;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;
using ScriptBee.Ports.Instance;
using ScriptBee.Rest.Api.Generated;
using ScriptBee.Rest.Api.Generated.Contracts;
using ScriptBee.Rest.Converters;
using PluginExtensionPoint = ScriptBee.Domain.Model.Plugins.Manifest.PluginExtensionPoint;
using RestPluginExtensionPoint = ScriptBee.Rest.Api.Generated.Contracts.PluginExtensionPoint;

namespace ScriptBee.Rest;

public class GetPluginsAdapter(IHttpClientFactory httpClientFactory) : IGetPlugins
{
    public async Task<IEnumerable<Plugin>> GetLoadedPlugins(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(instanceInfo.Url);

        var pluginsApi = GetAnalysisApiWithCustomConverter(client);

        var response = await pluginsApi.PluginsGet(cancellationToken);

        return response.Data.Select(MapPlugin);
    }

    private static IAnalysisApi GetAnalysisApiWithCustomConverter(HttpClient client)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        options.Converters.Insert(0, new JsonStringEnumMemberConverterFactory());

        var settings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(options),
        };
        var pluginsApi = RestService.For<IAnalysisApi>(client, settings);
        return pluginsApi;
    }

    private static Plugin MapPlugin(InstalledPlugin plugin)
    {
        return new Plugin(
            plugin.FolderPath,
            new PluginId(plugin.Id, new Version(plugin.Version)),
            MapManifest(plugin.Manifest)
        );
    }

    private static PluginManifest MapManifest(InstalledPluginManifest manifest)
    {
        var extensionPoints = manifest
            .ExtensionPoints.Select(MapExtensionPoint)
            .OfType<PluginExtensionPoint>()
            .ToList();

        return new PluginManifest
        {
            ApiVersion = manifest.ApiVersion,
            Name = manifest.Name,
            Description = manifest.Description,
            Author = manifest.Author,
            ExtensionPoints = extensionPoints,
        };
    }

    private static PluginExtensionPoint? MapExtensionPoint(RestPluginExtensionPoint ep)
    {
        return ep switch
        {
            PluginExtensionPointLoaderPluginExtensionPoint point => new LoaderPluginExtensionPoint
            {
                Kind = PluginKind.Loader,
                EntryPoint = point.EntryPoint,
                Version = point.Version,
            },
            PluginExtensionPointLinkerPluginExtensionPoint point => new LinkerPluginExtensionPoint
            {
                Kind = PluginKind.Linker,
                EntryPoint = point.EntryPoint,
                Version = point.Version,
            },
            PluginExtensionPointHelperFunctionsPluginExtensionPoint point =>
                new HelperFunctionsPluginExtensionPoint
                {
                    Kind = PluginKind.HelperFunctions,
                    EntryPoint = point.EntryPoint,
                    Version = point.Version,
                },
            PluginExtensionPointNestedPluginExtensionPoint point => new PluginBundleExtensionPoint
            {
                Kind = PluginKind.Plugin,
                EntryPoint = point.EntryPoint,
                Version = point.Version,
            },
            PluginExtensionPointScriptGeneratorPluginExtensionPoint point =>
                new ScriptGeneratorPluginExtensionPoint
                {
                    Kind = PluginKind.ScriptGenerator,
                    EntryPoint = point.EntryPoint,
                    Version = point.Version,
                    Language = point.Language,
                    Extension = point.Extension,
                },
            PluginExtensionPointScriptRunnerPluginExtensionPoint point =>
                new ScriptRunnerPluginExtensionPoint
                {
                    Kind = PluginKind.ScriptRunner,
                    EntryPoint = point.EntryPoint,
                    Version = point.Version,
                    Language = point.Language,
                    Extension = point.Extension,
                },
            PluginExtensionPointUiPluginExtensionPoint point => new UiPluginExtensionPoint
            {
                Kind = PluginKind.Ui,
                EntryPoint = point.EntryPoint,
                Version = point.Version,
                RemoteName = point.RemoteName,
                RemoteEntry = point.RemoteEntry,
                Outlets = MapOutlets(point.Outlets),
            },
            _ => null,
        };
    }

    private static IEnumerable<UiPluginExtensionPointOutlet> MapOutlets(
        IEnumerable<InstalledPluginExtensionPointOutletBase> outlets
    )
    {
        return outlets.Select(MapOutlet);
    }

    private static UiPluginExtensionPointOutlet MapOutlet(
        InstalledPluginExtensionPointOutletBase outlet
    )
    {
        return outlet switch
        {
            InstalledPluginExtensionPointOutletBaseInstalledPluginTopNavigationBarOutlet topNavigationBarOutlet =>
                new TopNavigationBarOutlet
                {
                    Type = OutletTypes.TopNavigationBar,
                    ExposedModule = topNavigationBarOutlet.ExposedModule,
                    Path = topNavigationBarOutlet.Path,
                    Label = topNavigationBarOutlet.Label,
                    Nested = topNavigationBarOutlet.Nested,
                    ComponentName = topNavigationBarOutlet.ComponentName,
                },
            InstalledPluginExtensionPointOutletBaseInstalledPluginSidePanelOutlet sidePanelOutlet =>
                new SidePanelOutlet
                {
                    Type = OutletTypes.SidePanel,
                    ExposedModule = sidePanelOutlet.ExposedModule,
                    Path = sidePanelOutlet.Path,
                    Label = sidePanelOutlet.Label,
                    Nested = sidePanelOutlet.Nested,
                    ComponentName = sidePanelOutlet.ComponentName,
                    Icon = sidePanelOutlet.Icon,
                },
            InstalledPluginExtensionPointOutletBaseInstalledPluginFilePreviewerOutlet filePreviewerOutlet =>
                new FilePreviewerOutlet
                {
                    Type = OutletTypes.FilePreviewer,
                    ExposedModule = filePreviewerOutlet.ExposedModule,
                    Label = filePreviewerOutlet.Label,
                    ComponentName = filePreviewerOutlet.ComponentName,
                    Icon = filePreviewerOutlet.Icon,
                    SupportedFileExtensions = filePreviewerOutlet.SupportedFileExtensions?.ToList(),
                },
            _ => new UiPluginExtensionPointOutlet { Type = "unknown" },
        };
    }
}
