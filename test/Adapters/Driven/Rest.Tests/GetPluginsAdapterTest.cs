using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Domain.Model.Project;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Rest.Tests;

public sealed class GetPluginsAdapterTest : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    private readonly GetPluginsAdapter _getPluginsAdapter = new(new DefaultHttpClientFactory());

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task ShouldReturnLoaderPlugin()
    {
        MockGetPlugins(
            """
            {
                "entryPoint": "entry-point",
                "kind": "Loader",
                "version": "1.2.3"
            }
            """
        );

        var response = await _getPluginsAdapter.GetLoadedPlugins(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            )
        );

        var plugin = response.Single();
        AssertPluginProperties(plugin);
        var pluginExtensionPoint = (
            plugin.Manifest.ExtensionPoints.Single() as LoaderPluginExtensionPoint
        )!;
        pluginExtensionPoint.EntryPoint.ShouldBe("entry-point");
        pluginExtensionPoint.Kind.ShouldBe(PluginKind.Loader);
        pluginExtensionPoint.Version.ShouldBe("1.2.3");
    }

    [Fact]
    public async Task ShouldReturnLinkerPlugin()
    {
        MockGetPlugins(
            """
            {
                "entryPoint": "entry-point",
                "kind": "Linker",
                "version": "1.2.3"
            }
            """
        );

        var response = await _getPluginsAdapter.GetLoadedPlugins(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            )
        );

        var plugin = response.Single();
        AssertPluginProperties(plugin);
        var pluginExtensionPoint = (
            plugin.Manifest.ExtensionPoints.Single() as LinkerPluginExtensionPoint
        )!;
        pluginExtensionPoint.EntryPoint.ShouldBe("entry-point");
        pluginExtensionPoint.Kind.ShouldBe(PluginKind.Linker);
        pluginExtensionPoint.Version.ShouldBe("1.2.3");
    }

    [Fact]
    public async Task ShouldReturnHelperFunctionsPlugin()
    {
        MockGetPlugins(
            """
            {
                "entryPoint": "entry-point",
                "kind": "HelperFunctions",
                "version": "1.2.3"
            }
            """
        );

        var response = await _getPluginsAdapter.GetLoadedPlugins(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            )
        );

        var plugin = response.Single();
        AssertPluginProperties(plugin);
        var pluginExtensionPoint = (
            plugin.Manifest.ExtensionPoints.Single() as HelperFunctionsPluginExtensionPoint
        )!;
        pluginExtensionPoint.EntryPoint.ShouldBe("entry-point");
        pluginExtensionPoint.Kind.ShouldBe(PluginKind.HelperFunctions);
        pluginExtensionPoint.Version.ShouldBe("1.2.3");
    }

    [Fact]
    public async Task ShouldReturnScriptGeneratorPlugin()
    {
        MockGetPlugins(
            """
            {
                "entryPoint": "entry-point",
                "kind": "ScriptGenerator",
                "version": "1.2.3",
                "language": "csharp",
                "extension": ".cs"
            }
            """
        );

        var response = await _getPluginsAdapter.GetLoadedPlugins(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            )
        );

        var plugin = response.Single();
        AssertPluginProperties(plugin);
        var pluginExtensionPoint = (
            plugin.Manifest.ExtensionPoints.Single() as ScriptGeneratorPluginExtensionPoint
        )!;
        pluginExtensionPoint.EntryPoint.ShouldBe("entry-point");
        pluginExtensionPoint.Kind.ShouldBe(PluginKind.ScriptGenerator);
        pluginExtensionPoint.Version.ShouldBe("1.2.3");
        pluginExtensionPoint.Language.ShouldBe("csharp");
        pluginExtensionPoint.Extension.ShouldBe(".cs");
    }

    [Fact]
    public async Task ShouldReturnScriptRunnerPlugin()
    {
        MockGetPlugins(
            """
            {
                "entryPoint": "entry-point",
                "kind": "ScriptRunner",
                "version": "1.2.3",
                "language": "csharp",
                "extension": ".cs"
            }
            """
        );

        var response = await _getPluginsAdapter.GetLoadedPlugins(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            )
        );

        var plugin = response.Single();
        AssertPluginProperties(plugin);
        var pluginExtensionPoint = (
            plugin.Manifest.ExtensionPoints.Single() as ScriptRunnerPluginExtensionPoint
        )!;
        pluginExtensionPoint.EntryPoint.ShouldBe("entry-point");
        pluginExtensionPoint.Kind.ShouldBe(PluginKind.ScriptRunner);
        pluginExtensionPoint.Version.ShouldBe("1.2.3");
        pluginExtensionPoint.Language.ShouldBe("csharp");
        pluginExtensionPoint.Extension.ShouldBe(".cs");
    }

    [Fact]
    public async Task ShouldReturnUiPlugin()
    {
        MockGetPlugins(
            """
            {
                "entryPoint": "entry-point",
                "kind": "UI",
                "version": "1.2.3",
                "port": 1234,
                "componentName": "component",
                "exposedModule": "module",
                "remoteEntry": "remote-entry",
                "uiPluginType": "type"
            }
            """
        );

        var response = await _getPluginsAdapter.GetLoadedPlugins(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            )
        );

        var plugin = response.Single();
        AssertPluginProperties(plugin);
        var pluginExtensionPoint = (
            plugin.Manifest.ExtensionPoints.Single() as UiPluginExtensionPoint
        )!;
        pluginExtensionPoint.EntryPoint.ShouldBe("entry-point");
        pluginExtensionPoint.Kind.ShouldBe(PluginKind.Ui);
        pluginExtensionPoint.Version.ShouldBe("1.2.3");
        pluginExtensionPoint.Port.ShouldBe(1234);
        pluginExtensionPoint.ComponentName.ShouldBe("component");
        pluginExtensionPoint.ExposedModule.ShouldBe("module");
        pluginExtensionPoint.RemoteEntry.ShouldBe("remote-entry");
        pluginExtensionPoint.UiPluginType.ShouldBe("type");
    }

    [Fact]
    public async Task GivenUnknownKind_ShouldReturnPluginWithoutExtensionPoints()
    {
        MockGetPlugins(
            """
            {
                "entryPoint": "entry-point",
                "kind": "unknown",
                "version": "1.2.3"
            }
            """
        );

        var response = await _getPluginsAdapter.GetLoadedPlugins(
            new InstanceInfo(
                new InstanceId(Guid.Empty),
                ProjectId.FromValue("id"),
                _server.Urls[0],
                DateTimeOffset.Now
            )
        );

        var plugin = response.Single();
        AssertPluginProperties(plugin);
        plugin.Manifest.ExtensionPoints.ShouldBeEmpty();
    }

    private void MockGetPlugins(string extensionPoint)
    {
        _server
            .Given(Request.Create().WithPath("/api/plugins").UsingGet())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBody(
                        $$"""
                        [
                            {
                                "folderPath": "folder",
                                "id": "id",
                                "version": "1.2.3",
                                "manifest":
                                {
                                    "apiVersion": "1.0.0",
                                    "name": "name",
                                    "author": "author",
                                    "description": "description",
                                    "extensionPoints":
                                    [
                                       {{extensionPoint}}
                                    ]
                                }
                            }
                        ]
                        """
                    )
            );
    }

    private static void AssertPluginProperties(Plugin plugin)
    {
        plugin.FolderPath.ShouldBe("folder");
        plugin.Id.ShouldBe("id");
        plugin.Version.ShouldBe(new Version("1.2.3"));
        plugin.Manifest.ApiVersion.ShouldBe("1.0.0");
        plugin.Manifest.Name.ShouldBe("name");
        plugin.Manifest.Author.ShouldBe("author");
        plugin.Manifest.Description.ShouldBe("description");
    }
}
