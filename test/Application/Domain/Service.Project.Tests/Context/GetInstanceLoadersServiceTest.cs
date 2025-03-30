using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using static ScriptBee.Tests.Common.InstanceInfoFixture;

namespace ScriptBee.Service.Project.Tests.Context;

public class GetInstanceLoadersServiceTest
{
    private readonly IGetPlugins _getPlugins = Substitute.For<IGetPlugins>();

    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly GetInstanceLoadersService _getInstanceLoadersService;

    public GetInstanceLoadersServiceTest()
    {
        _getInstanceLoadersService = new GetInstanceLoadersService(
            _getPlugins,
            _getProjectInstance
        );
    }

    [Fact]
    public async Task GivenNoInstance_ExpectEmtpyList()
    {
        var projectId = ProjectId.FromValue("id");
        var instanceId = new InstanceId("139ad4ad-1f4f-4006-863c-55bc76608d3c");
        var query = new GetLoadersQuery(projectId, instanceId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var loaders = await _getInstanceLoadersService.Get(query);

        loaders.ShouldBeEmpty();
    }

    [Fact]
    public async Task GivenInstanceLoaderPlugins_ExpectLoaders()
    {
        var projectId = ProjectId.FromValue("id");
        var instanceId = new InstanceId("4e125fff-589f-471a-91e6-3804b6ed701a");
        var query = new GetLoadersQuery(projectId, instanceId);
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );
        _getPlugins
            .GetLoadedPlugins(instanceInfo, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Plugin>>(
                    [
                        new Plugin(
                            "folder",
                            "loader-id",
                            new Version(),
                            new PluginManifest
                            {
                                Name = "Loader",
                                ExtensionPoints = [new LoaderPluginExtensionPoint()],
                            }
                        ),
                        new Plugin(
                            "folder",
                            "loader-id-multiple",
                            new Version(),
                            new PluginManifest
                            {
                                Name = "Multiple Loaders",
                                ExtensionPoints =
                                [
                                    new LoaderPluginExtensionPoint(),
                                    new LoaderPluginExtensionPoint(),
                                ],
                            }
                        ),
                        new Plugin(
                            "folder",
                            "loader-id-without-extension-point",
                            new Version(),
                            new PluginManifest
                            {
                                Name = "Linker",
                                ExtensionPoints = [new LinkerPluginExtensionPoint()],
                            }
                        ),
                    ]
                )
            );

        var loaders = await _getInstanceLoadersService.Get(query);

        loaders
            .ToList()
            .ShouldBe(
                [
                    new Loader("loader-id", "Loader"),
                    new Loader("loader-id-multiple", "Multiple Loaders"),
                ]
            );
    }
}
