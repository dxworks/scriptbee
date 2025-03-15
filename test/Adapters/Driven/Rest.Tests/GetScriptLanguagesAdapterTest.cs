using NSubstitute;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Rest.Tests;

public class GetScriptLanguagesAdapterTest
{
    private readonly IGetPlugins _getPlugins = Substitute.For<IGetPlugins>();

    private readonly GetScriptLanguagesAdapter _getScriptLanguagesAdapter;

    public GetScriptLanguagesAdapterTest()
    {
        _getScriptLanguagesAdapter = new GetScriptLanguagesAdapter(_getPlugins);
    }

    [Fact]
    public async Task GetScriptLanguage()
    {
        var instanceInfo = new InstanceInfo(
            new InstanceId("147542d3-95dc-4be9-bcd1-10f718af6d27"),
            ProjectId.FromValue("id"),
            "url",
            DateTimeOffset.Now
        );
        _getPlugins
            .GetLoadedPlugins(instanceInfo, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        new Plugin(
                            "folder",
                            "id",
                            new Version(),
                            new PluginManifest
                            {
                                ExtensionPoints =
                                [
                                    new ScriptRunnerPluginExtensionPoint
                                    {
                                        Language = "language",
                                        Extension = ".lang",
                                    },
                                ],
                            }
                        ),
                    ]
                )
            );

        var result = await _getScriptLanguagesAdapter.Get(instanceInfo, "language");

        result.AsT0.ShouldBe(new ScriptLanguage("language", ".lang"));
    }

    [Fact]
    public async Task GivenNoScriptRunnerExtensions_GetScriptLanguageDoesNotExistsError()
    {
        var instanceInfo = new InstanceInfo(
            new InstanceId("147542d3-95dc-4be9-bcd1-10f718af6d27"),
            ProjectId.FromValue("id"),
            "url",
            DateTimeOffset.Now
        );
        _getPlugins
            .GetLoadedPlugins(instanceInfo, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        new Plugin(
                            "folder",
                            "id",
                            new Version(),
                            new PluginManifest
                            {
                                ExtensionPoints = [new LoaderPluginExtensionPoint()],
                            }
                        ),
                    ]
                )
            );

        var result = await _getScriptLanguagesAdapter.Get(instanceInfo, "language");

        result.AsT1.ShouldBe(new ScriptLanguageDoesNotExistsError("language"));
    }

    [Fact]
    public async Task GivenNoScriptRunnerForThatLanguage_GetScriptLanguageDoesNotExistsError()
    {
        var instanceInfo = new InstanceInfo(
            new InstanceId("147542d3-95dc-4be9-bcd1-10f718af6d27"),
            ProjectId.FromValue("id"),
            "url",
            DateTimeOffset.Now
        );
        _getPlugins
            .GetLoadedPlugins(instanceInfo, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        new Plugin(
                            "folder",
                            "id",
                            new Version(),
                            new PluginManifest
                            {
                                ExtensionPoints =
                                [
                                    new ScriptRunnerPluginExtensionPoint
                                    {
                                        Language = "csharp",
                                        Extension = ".cs",
                                    },
                                ],
                            }
                        ),
                    ]
                )
            );

        var result = await _getScriptLanguagesAdapter.Get(instanceInfo, "language");

        result.AsT1.ShouldBe(new ScriptLanguageDoesNotExistsError("language"));
    }
}
