using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;
using Xunit.Abstractions;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Plugins;

public class GetInstalledPluginsEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/plugins";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnLoaderPlugin()
    {
        var useCase = Substitute.For<IGetInstalledPluginsUseCase>();
        useCase
            .Get(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        CreatePlugin(
                            new LoaderPluginExtensionPoint
                            {
                                EntryPoint = "entry-point",
                                Kind = PluginKind.Loader,
                                Version = "1.2.3",
                            }
                        ),
                    ]
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var installedPlugins = await response.ReadContentAsync<IEnumerable<JsonElement>>();
        var plugin = installedPlugins.Single();
        AssertPluginProperties(plugin);
        var extensionPoint = plugin
            .GetProperty("manifest")
            .GetProperty("extensionPoints")
            .EnumerateArray()
            .Single();
        AssertBasicExtensionPointProperties(extensionPoint, PluginKind.Loader);
    }

    [Fact]
    public async Task ShouldReturnLinkerPlugin()
    {
        var useCase = Substitute.For<IGetInstalledPluginsUseCase>();
        useCase
            .Get(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        CreatePlugin(
                            new LinkerPluginExtensionPoint
                            {
                                EntryPoint = "entry-point",
                                Kind = PluginKind.Linker,
                                Version = "1.2.3",
                            }
                        ),
                    ]
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var installedPlugins = await response.ReadContentAsync<IEnumerable<JsonElement>>();
        var plugin = installedPlugins.Single();
        AssertPluginProperties(plugin);
        var extensionPoint = plugin
            .GetProperty("manifest")
            .GetProperty("extensionPoints")
            .EnumerateArray()
            .Single();
        AssertBasicExtensionPointProperties(extensionPoint, PluginKind.Linker);
    }

    [Fact]
    public async Task ShouldReturnHelperFunctionsPlugin()
    {
        var useCase = Substitute.For<IGetInstalledPluginsUseCase>();
        useCase
            .Get(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        CreatePlugin(
                            new HelperFunctionsPluginExtensionPoint
                            {
                                EntryPoint = "entry-point",
                                Kind = PluginKind.HelperFunctions,
                                Version = "1.2.3",
                            }
                        ),
                    ]
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var installedPlugins = await response.ReadContentAsync<IEnumerable<JsonElement>>();
        var plugin = installedPlugins.Single();
        AssertPluginProperties(plugin);
        var extensionPoint = plugin
            .GetProperty("manifest")
            .GetProperty("extensionPoints")
            .EnumerateArray()
            .Single();
        AssertBasicExtensionPointProperties(extensionPoint, PluginKind.HelperFunctions);
    }

    [Fact]
    public async Task ShouldReturnScriptGeneratorPlugin()
    {
        var useCase = Substitute.For<IGetInstalledPluginsUseCase>();
        useCase
            .Get(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        CreatePlugin(
                            new ScriptGeneratorPluginExtensionPoint
                            {
                                EntryPoint = "entry-point",
                                Kind = PluginKind.ScriptGenerator,
                                Version = "1.2.3",
                                Extension = ".cs",
                                Language = "csharp",
                            }
                        ),
                    ]
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var installedPlugins = await response.ReadContentAsync<IEnumerable<JsonElement>>();
        var plugin = installedPlugins.Single();
        AssertPluginProperties(plugin);
        var extensionPoint = plugin
            .GetProperty("manifest")
            .GetProperty("extensionPoints")
            .EnumerateArray()
            .Single();
        AssertBasicExtensionPointProperties(extensionPoint, PluginKind.ScriptGenerator);
        extensionPoint.GetProperty("extension").GetString().ShouldBe(".cs");
        extensionPoint.GetProperty("language").GetString().ShouldBe("csharp");
    }

    [Fact]
    public async Task ShouldReturnScriptRunnerPlugin()
    {
        var useCase = Substitute.For<IGetInstalledPluginsUseCase>();
        useCase
            .Get(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        CreatePlugin(
                            new ScriptRunnerPluginExtensionPoint
                            {
                                EntryPoint = "entry-point",
                                Kind = PluginKind.ScriptRunner,
                                Version = "1.2.3",
                                Language = "csharp",
                                Extension = ".cs",
                            }
                        ),
                    ]
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var installedPlugins = await response.ReadContentAsync<IEnumerable<JsonElement>>();
        var plugin = installedPlugins.Single();
        AssertPluginProperties(plugin);
        var extensionPoint = plugin
            .GetProperty("manifest")
            .GetProperty("extensionPoints")
            .EnumerateArray()
            .Single();
        AssertBasicExtensionPointProperties(extensionPoint, PluginKind.ScriptRunner);
        extensionPoint.GetProperty("language").GetString().ShouldBe("csharp");
        extensionPoint.GetProperty("extension").GetString().ShouldBe(".cs");
    }

    [Fact]
    public async Task ShouldReturnUiPlugin()
    {
        var useCase = Substitute.For<IGetInstalledPluginsUseCase>();
        useCase
            .Get(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        CreatePlugin(
                            new UiPluginExtensionPoint
                            {
                                EntryPoint = "entry-point",
                                Kind = PluginKind.Ui,
                                Version = "1.2.3",
                                Port = 1234,
                                ComponentName = "component",
                                ExposedModule = "module",
                                RemoteEntry = "remote-entry",
                                UiPluginType = "type",
                            }
                        ),
                    ]
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var installedPlugins = await response.ReadContentAsync<IEnumerable<JsonElement>>();
        var plugin = installedPlugins.Single();
        AssertPluginProperties(plugin);
        var extensionPoint = plugin
            .GetProperty("manifest")
            .GetProperty("extensionPoints")
            .EnumerateArray()
            .Single();
        AssertBasicExtensionPointProperties(extensionPoint, PluginKind.Ui);
        extensionPoint.GetProperty("port").GetInt32().ShouldBe(1234);
        extensionPoint.GetProperty("componentName").GetString().ShouldBe("component");
        extensionPoint.GetProperty("exposedModule").GetString().ShouldBe("module");
        extensionPoint.GetProperty("remoteEntry").GetString().ShouldBe("remote-entry");
        extensionPoint.GetProperty("uiPluginType").GetString().ShouldBe("type");
    }

    private static Plugin CreatePlugin(PluginExtensionPoint extensionPoint)
    {
        return new Plugin(
            "folder",
            "id",
            new Version(1, 2, 3),
            new PluginManifest
            {
                ApiVersion = "1.0.0",
                Name = "name",
                Author = "author",
                Description = "description",
                ExtensionPoints = [extensionPoint],
            }
        );
    }

    private static void AssertBasicExtensionPointProperties(
        JsonElement extensionPoint,
        string? type
    )
    {
        extensionPoint.GetProperty("entryPoint").GetString().ShouldBe("entry-point");
        extensionPoint.GetProperty("kind").GetString().ShouldBe(type);
        extensionPoint.GetProperty("version").GetString().ShouldBe("1.2.3");
    }

    private static void AssertPluginProperties(JsonElement plugin)
    {
        plugin.GetProperty("id").GetString().ShouldBe("id");
        plugin.GetProperty("version").GetString().ShouldBe("1.2.3");
        plugin.GetProperty("manifest").GetProperty("apiVersion").GetString().ShouldBe("1.0.0");
        plugin.GetProperty("manifest").GetProperty("name").GetString().ShouldBe("name");
        plugin.GetProperty("manifest").GetProperty("author").GetString().ShouldBe("author");
        plugin
            .GetProperty("manifest")
            .GetProperty("description")
            .GetString()
            .ShouldBe("description");
    }
}
