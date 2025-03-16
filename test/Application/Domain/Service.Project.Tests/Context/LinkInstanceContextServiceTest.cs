using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Tests.Context;

public class LinkInstanceContextServiceTest
{
    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly ILinkInstanceContext _linkInstanceContext =
        Substitute.For<ILinkInstanceContext>();

    private readonly LinkInstanceContextService _linkInstanceContextService;

    public LinkInstanceContextServiceTest()
    {
        _linkInstanceContextService = new LinkInstanceContextService(
            _getProjectInstance,
            _linkInstanceContext
        );
    }

    [Fact]
    public async Task GivenInstance_ExpectContextToBeLinked()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("3cf3006c-234f-4d7b-951d-b17b5226020e");
        List<string> linkerIds = ["linker-id"];
        var command = new LinkContextCommand(projectId, instanceId, linkerIds);
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

        var result = await _linkInstanceContextService.Link(command);

        result.AsT0.ShouldBe(new Unit());
        await _linkInstanceContext
            .Received(1)
            .Link(instanceInfo, linkerIds, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenNoInstanceForInstanceId_ExpectInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("3cf3006c-234f-4d7b-951d-b17b5226020e");
        var command = new LinkContextCommand(projectId, instanceId, ["linker-id"]);
        var instanceDoesNotExistsError = new InstanceDoesNotExistsError(instanceId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    instanceDoesNotExistsError
                )
            );

        var result = await _linkInstanceContextService.Link(command);

        result.AsT1.ShouldBe(instanceDoesNotExistsError);
    }
}
