using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Service.Project.Context;
using ScriptBee.UseCases.Project.Context;
using static ScriptBee.Tests.Common.InstanceInfoFixture;

namespace ScriptBee.Service.Project.Tests.Context;

public class GetInstanceContextServiceTest
{
    private readonly IGetProjectInstance _getProjectInstance =
        Substitute.For<IGetProjectInstance>();

    private readonly IGetInstanceContext _getInstanceContext =
        Substitute.For<IGetInstanceContext>();

    private readonly GetInstanceContextService _getInstanceContextService;

    public GetInstanceContextServiceTest()
    {
        _getInstanceContextService = new GetInstanceContextService(
            _getProjectInstance,
            _getInstanceContext
        );
    }

    [Fact]
    public async Task GivenInstance_ExpectContextSlices()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("6143ee26-8150-43b4-b1c3-e57da86061b8");
        var query = new GetInstanceContextQuery(projectId, instanceId);
        var instanceInfo = BasicInstanceInfo(projectId);
        List<ContextSlice> contextSlices = [new("model", ["plugin-id"])];
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );
        _getInstanceContext
            .Get(instanceInfo, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<ContextSlice>>(contextSlices));

        var result = await _getInstanceContextService.Get(
            query,
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(contextSlices);
    }

    [Fact]
    public async Task GivenNoInstanceForInstanceId_ExpectInstanceDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("6143ee26-8150-43b4-b1c3-e57da86061b8");
        var query = new GetInstanceContextQuery(projectId, instanceId);
        var instanceDoesNotExistsError = new InstanceDoesNotExistsError(instanceId);
        _getProjectInstance
            .Get(instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    instanceDoesNotExistsError
                )
            );

        var result = await _getInstanceContextService.Get(
            query,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(instanceDoesNotExistsError);
    }
}
