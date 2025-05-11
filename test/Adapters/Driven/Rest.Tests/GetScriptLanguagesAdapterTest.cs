using NSubstitute;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Plugins;
using static ScriptBee.Tests.Common.InstanceInfoFixture;

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
        var instanceInfo = BasicInstanceInfo(ProjectId.FromValue("id"));
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

        var result = await _getScriptLanguagesAdapter.Get(
            instanceInfo,
            "language",
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(new ScriptLanguage("language", ".lang"));
    }

    [Fact]
    public async Task GivenNoScriptRunnerExtensions_GetScriptLanguageDoesNotExistsError()
    {
        var instanceInfo = BasicInstanceInfo(ProjectId.FromValue("id"));
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

        var result = await _getScriptLanguagesAdapter.Get(
            instanceInfo,
            "language",
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new ScriptLanguageDoesNotExistsError("language"));
    }

    [Fact]
    public async Task GivenNoScriptRunnerForThatLanguage_GetScriptLanguageDoesNotExistsError()
    {
        var instanceInfo = BasicInstanceInfo(ProjectId.FromValue("id"));
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

        var result = await _getScriptLanguagesAdapter.Get(
            instanceInfo,
            "language",
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new ScriptLanguageDoesNotExistsError("language"));
    }
}
