using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Tests.Context;

public class GetInstanceLinkersServiceTest
{
    private readonly IGetPlugins _getPlugins = Substitute.For<IGetPlugins>();

    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly GetInstanceLinkersService _getInstanceLinkersService;

    public GetInstanceLinkersServiceTest()
    {
        _getInstanceLinkersService = new GetInstanceLinkersService(
            _getPlugins,
            _getProjectInstance
        );
    }

    [Fact]
    public async Task GivenNoInstance_ExpectEmtpyList()
    {
        var projectId = ProjectId.FromValue("id");
        var instanceId = new InstanceId("db0ad7b2-440d-4c02-a935-0871e8197b83");
        var query = new GetLinkersQuery(projectId, instanceId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var linkers = await _getInstanceLinkersService.Get(query);

        linkers.ShouldBeEmpty();
    }

    [Fact]
    public async Task GivenInstanceLoaderPlugins_ExpectLinkers()
    {
        var projectId = ProjectId.FromValue("id");
        var instanceId = new InstanceId("db0ad7b2-440d-4c02-a935-0871e8197b83");
        var query = new GetLinkersQuery(projectId, instanceId);
        var instanceInfo = new InstanceInfo(
            new InstanceId(Guid.NewGuid()),
            projectId,
            "http://instance",
            DateTimeOffset.Now
        );
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
                            "linker-id",
                            new Version(),
                            new PluginManifest
                            {
                                Name = "Linker",
                                ExtensionPoints = [new LinkerPluginExtensionPoint()],
                            }
                        ),
                        new Plugin(
                            "folder",
                            "linker-id-multiple",
                            new Version(),
                            new PluginManifest
                            {
                                Name = "Multiple Linkers",
                                ExtensionPoints =
                                [
                                    new LinkerPluginExtensionPoint(),
                                    new LinkerPluginExtensionPoint(),
                                ],
                            }
                        ),
                        new Plugin(
                            "folder",
                            "linker-id-without-extension-point",
                            new Version(),
                            new PluginManifest
                            {
                                Name = "Loader",
                                ExtensionPoints = [new LoaderPluginExtensionPoint()],
                            }
                        ),
                    ]
                )
            );

        var linkers = await _getInstanceLinkersService.Get(query);

        linkers
            .ToList()
            .ShouldBe(
                [
                    new Linker("linker-id", "Linker"),
                    new Linker("linker-id-multiple", "Multiple Linkers"),
                ]
            );
    }
}
